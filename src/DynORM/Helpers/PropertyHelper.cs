using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using System.Linq.Expressions;
using Amazon.DynamoDBv2.Model;
using DynORM.Exceptions;

namespace DynORM.Helpers
{
    internal class PropertyHelper
    {
        private static volatile PropertyHelper _instance;
        private static object _syncRoot = new Object();
        

        private PropertyHelper()
        {

        }

        public static PropertyHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new PropertyHelper();
                    }
                }
                return _instance;
            }
        }

        public string GetColumnName(MemberInfo member)
        {
            var property = member.GetCustomAttribute<DynamoDBPropertyAttribute>();
            return property?.AttributeName ?? member.Name;
        }

        public string GetColumnName(MemberExpression property)
        {
            if(property?.NodeType != ExpressionType.MemberAccess)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            var memberInfo = property.Member;
            return GetColumnName(memberInfo);
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
