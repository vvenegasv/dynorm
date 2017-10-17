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
    internal class ItemMaker<TModel> where TModel : class
    {
        private readonly TModel _item;
        private readonly ItemHelper _itemHelper;
        private readonly MetadataHelper _metadataHelper;
        private readonly Dictionary<string, AttributeValue> _data;

        public ItemMaker(TModel item)
        {
            _item = item;
            _itemHelper = ItemHelper.Instance;
            _metadataHelper = MetadataHelper.Instance;
            _data = new Dictionary<string, AttributeValue>();
        }

        public Dictionary<string, AttributeValue> GetItem()
        {
            return new ReadOnlyDictionary<string, AttributeValue>(_data).ToDictionary(x => x.Key, x => x.Value);
        }

        private void Bind()
        {
            foreach (var property in _item.GetType().GetTypeInfo().GetProperties())
            {
                if(_itemHelper.ColumnIsIgnored(property))
                    continue;

                var value = property.GetValue(_item);
                var name = _itemHelper.GetColumnName(property);
                var converterType = _itemHelper.GetColumnConverter(property);
                var type = _itemHelper.GetColumnType(property);
                AttributeValue attributeValue = null;

                if (converterType != null)
                {
                    var converter = (IDynoConvert) Activator.CreateInstance(converterType);
                    var dynamoValue = converter.ToItem(value);
                    var propertyType = property.GetType();

                    if (_metadataHelper.IsList(propertyType))
                    {
                        if (_metadataHelper.GenericArgumentIsCustomObject(propertyType))
                        {
                            
                        }
                            
                    }

                        
                }
                else
                {
                    
                }
            }
        }

        
        private AttributeValue ToAttibuteValue<TModel>(TModel item, PropertyInfo property, Type converter)
        {
            var type = property.PropertyType;
            var value = new AttributeValue();


            if (_metadataHelper.IsStream(type))
            {
                value.B = (MemoryStream)property.GetValue(item);
            }
            else if (_metadataHelper.IsBoolean(type))
            {
                value.BOOL = Convert.ToBoolean(property.GetValue(item));
                value.IsBOOLSet = true;
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.GenericArgumentIsStream(type))
            {
                value.BS = (List<MemoryStream>)property.GetValue(item);
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.GenericArgumentIsString(type))
            {
                value.SS = (List<string>)property.GetValue(item);
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.GenericArgumentIsNumber(type))
            {
                value.NS = ((List<object>)property.GetValue(item))
                    .Select(x => Convert.ToString(x).Replace(",", "."))
                    .ToList();
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.GenericArgumentIsSingleValue(type))
            {
                /*
                value.L = ((List<dynamic>)property.GetValue(item))
                    .Select(x => ToAttibuteValue(item, property))
                    .ToList();
                value.IsLSet = true;
                */
            }
            else if (_metadataHelper.IsList(type) && _metadataHelper.IsCustomObject(type))
            {
                var childrens = new List<AttributeValue>();
                var castedList = property.GetValue(item) as IEnumerable<dynamic>;

                foreach (var child in castedList)
                {
                    /*
                    var childAttributeValue = new AttributeValue
                    {
                        M = ToDictionary(child)
                    };
                    childrens.Add(childAttributeValue);
                    */
                }
                value.L = childrens;
                value.IsLSet = true;
            }
            else if (!_metadataHelper.IsList(type) && _metadataHelper.IsCustomObject(type))
            {
                //value.M = ToDictionary(property.GetValue(item));
                value.IsMSet = true;
            }
            else if (_metadataHelper.IsNumber(type))
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
