using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using System.Linq.Expressions;
using Amazon.DynamoDBv2.Model;
using DynORM.Attributes;
using DynORM.Exceptions;
using DynORM.Models;

namespace DynORM.Helpers
{
    internal class ItemHelper
    {
        private static volatile ItemHelper _instance;
        private static object _syncRoot = new Object();
        private readonly MetadataHelper _metadata;

        private ItemHelper()
        {
            _metadata = MetadataHelper.Instance;
        }

        public static ItemHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new ItemHelper();
                    }
                }
                return _instance;
            }
        }

        public string GetColumnName(MemberInfo member)
        {
            var dynProp = member.GetCustomAttribute<DynamoDBPropertyAttribute>();
            var dynHashProp = member.GetCustomAttribute<DynamoDBHashKeyAttribute>();
            var dynRangeProp = member.GetCustomAttribute<DynamoDBRangeKeyAttribute>();
            var dynGsiHashProp = member.GetCustomAttribute<DynamoDBGlobalSecondaryIndexHashKeyAttribute>();
            var dynGsiRangeProp = member.GetCustomAttribute<DynamoDBGlobalSecondaryIndexRangeKeyAttribute>();

            if (!string.IsNullOrWhiteSpace(dynProp?.AttributeName))
                return dynProp.AttributeName;
            if (!string.IsNullOrWhiteSpace(dynHashProp?.AttributeName))
                return dynHashProp.AttributeName;
            if (!string.IsNullOrWhiteSpace(dynRangeProp?.AttributeName))
                return dynRangeProp.AttributeName;
            if (!string.IsNullOrWhiteSpace(dynGsiHashProp?.AttributeName))
                return dynGsiHashProp.AttributeName;
            if (!string.IsNullOrWhiteSpace(dynGsiRangeProp?.AttributeName))
                return dynGsiRangeProp.AttributeName;
            return member.Name;
        }

        public string GetColumnName(MemberExpression property)
        {
            if(property?.NodeType != ExpressionType.MemberAccess)
                throw new ExpressionNotSupportedException($"Expression '{property}' not refers to a valid property");

            var memberInfo = property.Member;
            return GetColumnName(memberInfo);
        }

        public Type GetColumnConverter(MemberExpression property)
        {
            if (property?.NodeType != ExpressionType.MemberAccess)
                throw new ExpressionNotSupportedException($"Expression '{property}' not refers to a valid property");

            var memberInfo = property.Member;

            var dynProp = memberInfo.GetCustomAttribute<DynamoDBPropertyAttribute>();
            var dynHashProp = memberInfo.GetCustomAttribute<DynamoDBHashKeyAttribute>();
            var dynRangeProp = memberInfo.GetCustomAttribute<DynamoDBRangeKeyAttribute>();
            var dynGsiHashProp = memberInfo.GetCustomAttribute<DynamoDBGlobalSecondaryIndexHashKeyAttribute>();
            var dynGsiRangeProp = memberInfo.GetCustomAttribute<DynamoDBGlobalSecondaryIndexRangeKeyAttribute>();
            Type converter = null;

            if (dynProp?.Converter != null)
                converter = dynProp.Converter;
            if (dynHashProp?.Converter != null)
                converter = dynHashProp.Converter;
            if (dynRangeProp?.Converter != null)
                converter = dynRangeProp.Converter;
            if (dynGsiHashProp?.Converter != null)
                converter = dynGsiHashProp.Converter;
            if (dynGsiRangeProp?.Converter != null)
                converter = dynGsiRangeProp.Converter;

            if (converter == null)
                return converter;

            if (converter.GetTypeInfo().ImplementedInterfaces?.All(x => x != typeof(IPropertyConverter)) ?? true)
                throw new InvalidConverter($"The converter type '{converter}' not implements 'IPropertyConverter' interface");

            return converter;
        }

        public PropertyType GetColumnType(MemberExpression property)
        {
            if (property?.NodeType != ExpressionType.MemberAccess)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            var memberInfo = property.Member;
            var att = memberInfo.GetCustomAttribute<PropertyTypeAttribute>();

            if (att?.PropertyType != null)
                return att.PropertyType;
            if (_metadata.IsNumber(property.Type))
                return PropertyType.Number;
            if (_metadata.IsBoolean(property.Type))
                return PropertyType.Boolean;
            return PropertyType.String;

        }

        public string GetTableName<TModel>(TModel item) where TModel : class
        {
            var type = item.GetType();
            var attribute = type.GetTypeInfo().GetCustomAttribute<DynamoDBTableAttribute>();
            if(attribute != null)
                if (!string.IsNullOrWhiteSpace(attribute.TableName))
                    return attribute.TableName;

            return type.Name;
        }
    }
}
