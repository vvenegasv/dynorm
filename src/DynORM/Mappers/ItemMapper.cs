using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DynORM.Helpers;
using DynORM.Interfaces;
using DynORM.Models;

namespace DynORM.Mappers
{
    internal class ItemMapper
    {
        private static volatile ItemMapper _instance;
        private static object _syncRoot = new Object();
        private readonly ItemHelper _itemHelper;
        private readonly MetadataHelper _metadataHelper;

        private ItemMapper()
        {   
            _itemHelper = ItemHelper.Instance;
            _metadataHelper = MetadataHelper.Instance;
        }

        public static ItemMapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new ItemMapper();
                    }
                }
                return _instance;
            }
        }

        public Dictionary<string, AttributeValue> ToItem<TModel>(TModel item) where TModel : class
        {
            return ToDictionary(item);
        }

        public KeyValuePair<string, AttributeValue> ToValue(KeyValuePair<string, Tuple<object, PropertyType, Type>> item)
        {
            
        }

        private Dictionary<string, AttributeValue> ToDictionary<TItem>(TItem item)
        {
            var data = new Dictionary<string, AttributeValue>();

            foreach (var property in item.GetType().GetTypeInfo().GetProperties())
            {
                if (_itemHelper.ColumnIsIgnored(property))
                    continue;

                var name = _itemHelper.GetColumnName(property);
                var converterType = _itemHelper.GetColumnConverter(property);

                if (converterType != null)
                {
                    var value = property.GetValue(item);
                    var converter = (IDynoConvert)Activator.CreateInstance(converterType);
                    data.Add(name, converter.ToItem(value));
                }
                else
                {
                    data.Add(name, ToAttibuteValue(item, property));
                }
            }

            return data;
        }
        

        private AttributeValue ToAttibuteValue(Type type, object propertyValue)
        {
            var value = new AttributeValue();


            if (_metadataHelper.IsStream(type))
            {
                value.B = (MemoryStream)propertyValue;
            }
            else if (_metadataHelper.IsBoolean(type))
            {
                value.BOOL = Convert.ToBoolean(propertyValue);
                value.IsBOOLSet = true;
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.GenericArgumentIsStream(type))
            {
                value.BS = (List<MemoryStream>)propertyValue;
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.GenericArgumentIsString(type))
            {
                value.SS = (List<string>)propertyValue;
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.GenericArgumentIsNumber(type))
            {
                value.NS = ((List<object>)propertyValue)
                    .Select(x => Convert.ToString(x).Replace(",", "."))
                    .ToList();
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.GenericArgumentIsSingleValue(type))
            {
                value.L = ((List<object>)propertyValue)
                    .Select(x => ToAttibuteValue(x, x.GetType(), x))
                    .ToList();
                value.IsLSet = true;
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.IsCustomObject(type))
            {
                var childrens = new List<AttributeValue>();
                var castedList = propertyValue as IEnumerable<dynamic>;

                foreach (var child in castedList)
                {
                    var childAttributeValue = new AttributeValue
                    {
                        M = ToDictionary(child),
                        IsMSet = true
                    };
                    childrens.Add(childAttributeValue);
                }
                value.L = childrens;
                value.IsLSet = true;
            }
            else if (!_metadataHelper.IsList(type) && _metadataHelper.IsCustomObject(type))
            {
                value.M = ToDictionary(propertyValue);
                value.IsMSet = true;
            }
            else if (_metadataHelper.IsNumber(type))
            {
                value.N = Convert.ToString(propertyValue).Replace(",", ".");
            }
            else if (propertyValue.Equals(null))
            {
                value.NULL = true;
            }
            else
            {
                value.S = Convert.ToString(propertyValue);
            }

            return value;
        }

        private AttributeValue ToAttributeValue(Tuple<object, PropertyType, Type> value)
        {
            if (value.Item1 == null)
                return new AttributeValue {NULL = true};

            if (value.Item3 != null)
            {
                var converter = (IDynoConvert)Activator.CreateInstance(value.Item3);
                return converter.ToItem(value.Item1);
            }

            return ToAttibuteValue()
            
        }
    }
}
