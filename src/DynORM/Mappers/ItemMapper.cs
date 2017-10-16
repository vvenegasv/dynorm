﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DynORM.Helpers;

namespace DynORM.Mappers
{
    internal class ItemMapper<TItem> where TItem : class
    {
        private readonly MetadataHelper _metadata;
        private readonly ItemHelper _itemHelper;
        private readonly Dictionary<string, string> _names;
        private readonly TItem _item;
        
        public ItemMapper(TItem item)
        {
            _item = item;
            _itemHelper = ItemHelper.Instance;
            _metadata = MetadataHelper.Instance;
            _names = new Dictionary<string, string>();
        }

        public PutItemRequest ToRequest()
        {
            return new PutItemRequest
            {
                TableName = _itemHelper.GetTableName(_item),
                Item = ToDictionary(_item),
                ExpressionAttributeNames = _names
            };
        }


        private Dictionary<string, AttributeValue> ToDictionary<TModel>(TModel item) where TModel : class
        {
            var typeInfo = item.GetType().GetTypeInfo();
            var data = new Dictionary<string, AttributeValue>();

            foreach (var property in typeInfo.GetProperties())
            {
                var dynIgnoreProp = property.GetCustomAttribute<DynamoDBIgnoreAttribute>();
                if(dynIgnoreProp != null)
                    continue;

                var dynProp = property.GetCustomAttribute<DynamoDBPropertyAttribute>();
                var dynHashProp = property.GetCustomAttribute<DynamoDBHashKeyAttribute>();
                var dynRangeProp = property.GetCustomAttribute<DynamoDBRangeKeyAttribute>();
                var dynGsiHashProp = property.GetCustomAttribute<DynamoDBGlobalSecondaryIndexHashKeyAttribute>();
                var dynGsiRangeProp = property.GetCustomAttribute<DynamoDBGlobalSecondaryIndexRangeKeyAttribute>();

                string name;
                var value = ToAttibuteValue(item, property);

                if (!string.IsNullOrWhiteSpace(dynProp?.AttributeName))
                    name = dynProp.AttributeName;
                else if (!string.IsNullOrWhiteSpace(dynHashProp?.AttributeName))
                    name = dynHashProp.AttributeName;
                else if (!string.IsNullOrWhiteSpace(dynRangeProp?.AttributeName))
                    name = dynRangeProp.AttributeName;
                else if (!string.IsNullOrWhiteSpace(dynGsiHashProp?.AttributeName))
                    name = dynGsiHashProp.AttributeName;
                else if (!string.IsNullOrWhiteSpace(dynGsiRangeProp?.AttributeName))
                    name = dynGsiRangeProp.AttributeName;
                else
                    name = property.Name;

                var alias = "#" + name;
                data.Add(alias, value);
                if(!_names.ContainsKey(alias))
                    _names.Add(alias, name);
            }

            return data;
        }


        private AttributeValue ToAttibuteValue<TModel>(TModel item, PropertyInfo property)
        {
            var type = property.PropertyType;
            var value = new AttributeValue();


            if (_metadata.IsStream(type))
            {
                value.B = (MemoryStream) property.GetValue(item);
            }
            else if (_metadata.IsBoolean(type))
            {
                value.BOOL = Convert.ToBoolean(property.GetValue(item));
                value.IsBOOLSet = true;
            }
            else if (_metadata.IsList(type) && _metadata.GenericArgumentIsStream(type))
            {
                value.BS = (List<MemoryStream>) property.GetValue(item);
            }
            else if (_metadata.IsList(type) && _metadata.GenericArgumentIsString(type))
            {
                value.SS = (List<string>) property.GetValue(item);
            }
            else if (_metadata.IsList(type) && _metadata.GenericArgumentIsNumber(type))
            {
                value.NS = ((List<object>) property.GetValue(item))
                    .Select(x => Convert.ToString(x).Replace(",", "."))
                    .ToList();
            }
            else if (_metadata.IsList(type) && _metadata.GenericArgumentIsSingleValue(type))
            {
                value.L = ((List<dynamic>)property.GetValue(item))
                    .Select(x => ToAttibuteValue(item, property))
                    .ToList();
                value.IsLSet = true;
            }
            else if (_metadata.IsList(type) && _metadata.IsCustomObject(type))
            {
                var childrens = new List<AttributeValue>();
                var castedList = property.GetValue(item) as IEnumerable<dynamic>;

                foreach (var child in castedList)
                {
                    var childAttributeValue = new AttributeValue
                    {
                        M = ToDictionary(child)
                    };
                    childrens.Add(childAttributeValue);
                }
                value.L = childrens;
                value.IsLSet = true;
            }
            else if (!_metadata.IsList(type) && _metadata.IsCustomObject(type))
            {
                value.M = ToDictionary(property.GetValue(item));
                value.IsMSet = true;
            }
            else if (_metadata.IsNumber(type))
            {
                value.N = Convert.ToString(property.GetValue(item)).Replace(",", ".");
            }
            else if (property.GetValue(item).Equals(null))
            {
                value.NULL = true;
            }
            else
            {
                value.S = Convert.ToString(property.GetValue(item));
            }

            return value;
        }
    }
}

