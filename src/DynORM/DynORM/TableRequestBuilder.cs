using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using DynORM.Attributes;
using DynORM.Helpers;
using DynORM.InternalModels;
using DynORM.Models;

namespace DynORM
{
    internal class TableRequestBuilder<TModel> where TModel: class
    {
        private readonly Dictionary<string, KeyInfo> _keyInfos;
        private readonly string _tableName;
        

        public TableRequestBuilder(string enviromentPrefix)
        {
            _keyInfos = GetKeyInfos();
            _tableName = GetTableName(enviromentPrefix);
        }

        public string GetLogicalOperator(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.And:
                    return "AND";
                case ExpressionType.Or:
                    return "OR";
                case ExpressionType.Not:
                    return "NOT";
                default:
                    throw new NotSupportedException($"The expression type '{expressionType}' is not supported");
            }
        }

        public string GetComparisonOperator(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                default:
                    throw new NotSupportedException($"The expression type '{expressionType}' is not supported");
            }
        }

        private Dictionary<string, KeyInfo> GetKeyInfos()
        {
            var type = typeof(TModel).GetTypeInfo();
            var data = new List<dynamic>();
            var keyInfos = new Dictionary<string, KeyInfo>();

            foreach (var prop in type.GetProperties())
            {
                var hashKey = prop.GetCustomAttribute<DynamoDBHashKeyAttribute>();
                var rangeKey = type.GetCustomAttribute<DynamoDBRangeKeyAttribute>();

                if (hashKey != null)
                    data.Add(new
                    {
                        KeyName = "PK",
                        ColumnName = hashKey.AttributeName ?? prop.Name,
                        PropertyType = GetPropertyType(prop),
                        ColumnType = ColumnType.HashKey,
                        KeyType = KeyType.PrimaryKey
                    });
                else if (rangeKey != null)
                    data.Add(new
                    {
                        KeyName = "PK",
                        ColumnName = hashKey.AttributeName ?? prop.Name,
                        PropertyType = GetPropertyType(prop),
                        ColumnType = ColumnType.RangeKey,
                        KeyType = KeyType.PrimaryKey
                    });
            }

            foreach (var keyName in data.Select(x => x.KeyName).Distinct())
            {
                var columns = data.Where(x => x.KeyName == keyName);
                keyInfos[keyName] = new KeyInfo
                {
                    KeyType = columns.FirstOrDefault().KeyType,
                    KeyName = keyName,
                    Columns = columns.ToDictionary(x => (ColumnType) x.ColumnType, x => new ColumnInfo
                    {
                        ColumnType = x.ColumnType,
                        Name = x.ColumnName,
                        PropertyType = x.PropertyType
                    })
                };
            }

            return keyInfos;
        }

        private string GetTableName(string enviromentPrefix)
        {
            var tableType = typeof(TModel).GetTypeInfo();
            var tableName = string.Empty;

            var dynamoAttribute = (DynamoDBTableAttribute)tableType.GetCustomAttribute(typeof(DynamoDBTableAttribute));
            if (dynamoAttribute != null)
                tableName = dynamoAttribute.TableName;
            else
                tableName = tableType.Name;

            if (!string.IsNullOrWhiteSpace(enviromentPrefix))
            {
                enviromentPrefix = enviromentPrefix.Trim();
                if (!enviromentPrefix.EndsWith("-") && !enviromentPrefix.EndsWith("_") && !enviromentPrefix.EndsWith("."))
                    enviromentPrefix += "-";
                tableName = enviromentPrefix + tableName;
            }

            return tableName;
        }

        private PropertyType GetPropertyType(PropertyInfo property)
        {
            var customType = property.GetCustomAttribute<PropertyTypeAttribute>();
            if (customType != null)
                return customType.PropertyType;

            var helper = MetadataHelper.Instance;
            var type = property.PropertyType;
            if (helper.IsNumber(type))
                return PropertyType.Number;
            else if (typeof(bool) == type)
                return PropertyType.Boolean;
            return PropertyType.String;
        }
    }
}
