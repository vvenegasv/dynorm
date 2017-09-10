using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using DynORM.Attributes;
using DynORM.Models;

namespace DynORM.Helpers
{
    internal class MetadataHelper
    {
        private static volatile MetadataHelper _instance;
        private static object _syncRoot = new Object();
        private readonly IEnumerable<Type> _numberTypes = new List<Type>()
        {
            typeof(Int16), typeof(Int32), typeof(Int64),
            typeof(UInt16), typeof(UInt32), typeof(UInt64),
            typeof(decimal), typeof(float)
        };

        private MetadataHelper()
        {
            
        }

        public static MetadataHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new MetadataHelper();
                    }
                }
                return _instance;
            }
        }

        public string GetTableName<TModel>() where TModel : class
        {
            var tableType = typeof(TModel).GetTypeInfo();
            if(tableType == null)
                throw new NullReferenceException("The TModel reference for dynamo table does not exists");

            var dynamoAttribute = (DynamoDBTableAttribute)tableType.GetCustomAttribute(typeof(DynamoDBTableAttribute));
            if (dynamoAttribute != null)
                return dynamoAttribute.TableName;

            return tableType.Name;
        }

        public bool IsNumber(Type type)
        {
            if (_numberTypes.Contains(type))
                return true;
            
            return false;
        }
    }
}
