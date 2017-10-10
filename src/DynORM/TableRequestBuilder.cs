using System;
using System.Collections;
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
        private readonly PropertyHelper _propHelper = PropertyHelper.Instance;



        public TableRequestBuilder(string enviromentPrefix)
        {
            _keyInfos = GetKeyInfos();
            _tableName = GetTableName(enviromentPrefix);
        }

        /// <summary>
        /// Convert a expression to a valid DynamoDB query string
        /// </summary>
        /// <param name="expression">Expression to build</param>
        /// <exception cref="ArgumentNullException">if expression parameter is null</exception>
        /// <exception cref="ArgumentException">if expression.Body is not supported</exception>
        /// <returns>String with DynamoDB query string</returns>
        public string BuildExpression(Expression<Func<TModel, bool>> expression)
        {   
            if(expression?.Body is BinaryExpression)
                return BuildBinaryExpression((BinaryExpression)expression.Body);
            else if (expression?.Body is MethodCallExpression)
                return BuildMethodCallExpression(expression);
            else if (expression?.Body == null)
                throw new ArgumentNullException(nameof(expression), "The expression cannot be null");
            else
                throw new ArgumentException($"The expression '{expression?.Body}' is unsupported");
        }

        public string BuildBinaryExpression(BinaryExpression body)
        {
            var left = body.Left;
            var right = body.Right;
            var operationType = body.NodeType;

            return GetValue(left) + " " +
                   GetLogicalOperator(operationType) + " " +
                   GetValue(right);
        }

        public string BuildMethodCallExpression(Expression<Func<TModel, bool>> expression)
        {
             var body = expression.Body as MethodCallExpression;

            if (body?.Method == null)
                throw new ArgumentNullException(nameof(body), "The MethodCallExpression cannot be null");
            else if (!typeof(IEnumerable).IsAssignableFrom(body.Method.DeclaringType))
                throw new ArgumentException($"Expression '{body}' is unsupported");
            else if (body.Method.Name != "Contains")
                throw new ArgumentException($"Expression '{body}' is unsupported");
            
            var argument = body.Arguments.FirstOrDefault() as MemberExpression;

            if(argument == null)
                throw new InvalidCastException($"The expression '{body.Arguments}' is not assignable for MemberExpression");

            var compiled = expression.Compile();

            var target = compiled.Target;
            
            //var constants = (compiled.Target as System.Runtime.CompilerServices.Closure).Constants;

            //var countMethod = body.Method.DeclaringType.GetMethod("Count");
            

            return $"{_propHelper.GetColumnName(argument.Member)} in ";
        }

        public string GetLogicalOperator(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.And:
                    return "AND";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Or:
                    return "OR";
                case ExpressionType.Not:
                    return "NOT";


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
        
        public string GetValue(Expression expression)
        {
            BinaryExpression binaryExpression;
            if (MetadataHelper.Instance.TryCastTo(expression, out binaryExpression))
                return BuildBinaryExpression(binaryExpression);
            
            if (expression.NodeType == ExpressionType.MemberAccess)
                return ((MemberExpression)expression).Member.Name;

            if (expression.NodeType == ExpressionType.Constant)
                return (string)(expression as ConstantExpression).Value;

            return string.Empty;
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
