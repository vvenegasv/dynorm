using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;
using DynORM.Helpers;

namespace DynORM.Mappers
{
    internal class AttributeValueMapper
    {
        private static volatile AttributeValueMapper _instance;
        private static object _syncRoot = new Object();
        private readonly MetadataHelper _metadata;

        private AttributeValueMapper()
        {
            _metadata = MetadataHelper.Instance;
        }

        public static AttributeValueMapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new AttributeValueMapper();
                    }
                }
                return _instance;
            }
        }

        public AttributeValue ToAttributeValue(object value, Type type)
        {
            if (_metadata.IsNumber(type))
                return new AttributeValue {N = value};
            if (_metadata.IsBoolean(type))
                return new AttributeValue{ BOOL = Boolean.Parse(value), IsBOOLSet = true};
            
            
        }
    }
}
