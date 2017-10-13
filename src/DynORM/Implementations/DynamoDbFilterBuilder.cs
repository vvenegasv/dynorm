using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using DynORM.Enums;
using DynORM.Exceptions;
using DynORM.Helpers;
using DynORM.Interfaces;
using DynORM.Models;

namespace DynORM.Implementations
{
    public class DynamoDbFilterBuilder<TModel> : IFilterUsable, IFilterable<TModel> where TModel : class
    {
        private readonly PropertyHelper _propertyHelper;

        /// <summary>
        /// Dictionary Key=Parameter, Value=Tuple[Object Value, Destination Type, Convert Type]
        /// </summary>
        private readonly IDictionary<string, Tuple<object, PropertyType, Type>> _valuesTokens;
        private readonly IList<string> _filters;
        private readonly IDictionary<string, string> _namesTokens;


        public DynamoDbFilterBuilder() : base()
        {
            _propertyHelper = PropertyHelper.Instance;
            _filters = new List<string>();
            _namesTokens = new Dictionary<string, string>();
            _valuesTokens = new Dictionary<string, Tuple<object, Type, Type>>();
        }

        public DynamoDbFilterBuilder(string filter)
        {
            _filters = new List<string>();
            _filters.Add(filter);
        }



        public IFilterable<TModel> Where(Expression<Func<TModel, bool>> expression)
        {
            return Where(expression, FilterConcatenationType.And);
        }

        public IFilterable<TModel> Where(Expression<Func<TModel, bool>> expression, FilterConcatenationType concatenationType)
        {
            if (expression?.Body is BinaryExpression)
            {                
                if (_filters.Any())
                    _filters.Add($"{GetConcatenationValue(concatenationType)} {BuildBinaryExpression((BinaryExpression)expression.Body)}");
                else
                    _filters.Add(BuildBinaryExpression((BinaryExpression)expression.Body));
            }
            else
                throw new ExpressionNotSupportedException($"Expression {expression} is not supported. You can check the current documentation at https://github.com/vvenegasv/dynorm ");

            return this;
        }

        public IFilterable<TModel> Where(IFilterable<TModel> filter)
        {
            return Where(filter, FilterConcatenationType.And);
        }

        public IFilterable<TModel> Where(IFilterable<TModel> filter, FilterConcatenationType concatenationType)
        {
            if(filter == null)
                throw new ArgumentNullException(nameof(filter), $"{nameof(filter)} cannot be null");

            var filterBuilded = filter.Build();
            var names = filterBuilded.GetNames();
            var values = filterBuilded.GetValues();
            var query = filterBuilded.GetQuery();

            foreach (var name in names)
                if (!_namesTokens.ContainsKey(name.Key))
                    _namesTokens.Add(name.Key, name.Value);
            
            for(int index = 1; index<=values.Count; index++)
            {
                var originalKey = ":p" + index;
                var valuesCount = _valuesTokens.Count + 1;
                var tempKey = ":t" + valuesCount;
                var newKey = ":p" + valuesCount;
                var value = values[originalKey];
                _valuesTokens.Add(newKey, value);
                query = query.Replace(originalKey, tempKey);                
            }

            query = query.Replace(":t", ":p");
            
            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} ({query})");
            else
                _filters.Add($"({query})");

            return this;
        }

        public IFilterable<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values) where TValue : class
        {
            return WhereIn(property, values, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values, FilterConcatenationType concatenationType) where TValue : class
        {
            if (property?.NodeType != ExpressionType.Lambda)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");
                        
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            string filter = string.Empty;
            if (_filters.Any())
                filter = GetConcatenationValue(concatenationType);
            var columnName = _propertyHelper.GetColumnName(memberExpression);
            var columnAlias = "#" + columnName;
            if (!_namesTokens.ContainsKey(columnAlias))
                _namesTokens.Add(columnAlias, columnName);

            var parameters = new List<string>();
            filter += $" {columnAlias} in (";
            
            foreach (var val in values)
            {
                var index = _valuesTokens.Count + 1;
                var param = ":p" + index;
                parameters.Add(param);
                _valuesTokens.Add(param, new Tuple<object, Type, Type>(val, val.GetType(), null));
            }

            filter += string.Join(", ", parameters);
            filter += ")";
            _filters.Add(filter.Trim());

            return this;
        }

        public IFilterable<TModel> WhereAttributeExists<TValue>(Expression<Func<TModel, TValue>> property) where TValue : class
        {
            return WhereAttributeExists(property, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereAttributeExists<TValue>(Expression<Func<TModel, TValue>> property, FilterConcatenationType concatenationType) where TValue : class
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            var name = _propertyHelper.GetColumnName(memberExpression);
            var key = "#" + name;
            if (!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, name);

            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} attribute_exists ({key})");
            else
                _filters.Add($"attribute_exists ({key})");

            return this;
        }

        public IFilterable<TModel> WhereAttributeExists(string property)
        {
            return WhereAttributeExists(property, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereAttributeExists(string property, FilterConcatenationType concatenationType)
        {
            var key = "#" + property;
            if(!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, property);

            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} attribute_exists ({key})");
            else
                _filters.Add($"attribute_exists ({key})");

            return this;
        }

        public IFilterable<TModel> WhereAttributeNotExists<TValue>(Expression<Func<TModel, TValue>> property) where TValue : class
        {
            return WhereAttributeNotExists(property, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereAttributeNotExists<TValue>(Expression<Func<TModel, TValue>> property, FilterConcatenationType concatenationType) where TValue : class
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            var name = _propertyHelper.GetColumnName(memberExpression);
            var key = "#" + name;
            if (!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, name);

            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} attribute_not_exists ({key})");
            else
                _filters.Add($"attribute_not_exists ({key})");

            return this;
        }

        public IFilterable<TModel> WhereAttributeNotExists(string property)
        {
            return WhereAttributeNotExists(property, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereAttributeNotExists(string property, FilterConcatenationType concatenationType)
        {
            var key = "#" + property;
            if (!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, property);

            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} attribute_not_exists ({key})");
            else
                _filters.Add($"attribute_not_exists ({key})");

            return this;
        }

        public IFilterable<TModel> WhereBeginsWith<TValue>(Expression<Func<TModel, TValue>> property, string substring) where TValue : class
        {
            return WhereBeginsWith(property, substring, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereBeginsWith<TValue>(Expression<Func<TModel, TValue>> property, string substring, FilterConcatenationType concatenationType) where TValue : class
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            var name = _propertyHelper.GetColumnName(memberExpression);
            var key = "#" + name;
            if (!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, name);

            var index = _valuesTokens.Count + 1;
            var parameter = ":p" + index;
            _valuesTokens.Add(parameter, new Tuple<object, Type, Type>(substring, substring.GetType(), null));

            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} begins_with ({key}, {parameter})");
            else
                _filters.Add($"begins_with ({key}, {parameter})");

            return this;
        }

        public IFilterable<TModel> WhereContains<TValue>(Expression<Func<TModel, TValue>> property, string target) where TValue : class
        {
            return WhereContains(property, target, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereContains<TValue>(Expression<Func<TModel, TValue>> property, string target, FilterConcatenationType concatenationType) where TValue : class
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            var name = _propertyHelper.GetColumnName(memberExpression);
            var key = "#" + name;
            if (!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, name);

            var index = _valuesTokens.Count + 1;
            var parameter = ":p" + index;
            _valuesTokens.Add(parameter, new Tuple<object, Type, Type>(target, target.GetType(), null));

            var query = $"contains ({key}, {parameter})";

            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} {query}");
            else
                _filters.Add(query);

            return this;
        }
        
        public IFilterable<TModel> WhereSize<TValue>(Expression<Func<TModel, TValue>> property, ComparisonType comparisonType, int value) where TValue : class
        {
            return WhereSize(property, comparisonType, value, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereSize<TValue>(Expression<Func<TModel, TValue>> property, ComparisonType comparisonType, int value, FilterConcatenationType concatenationType) where TValue : class
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            var name = _propertyHelper.GetColumnName(memberExpression);
            var key = "#" + name;
            if (!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, name);

            var index = _valuesTokens.Count + 1;
            var parameter = ":p" + index;
            _valuesTokens.Add(parameter, new Tuple<object, Type, Type>(value, value.GetType(), null));

            var query = $"size ({key}) {GetLogicalOperator(comparisonType)} {parameter})";

            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} {query}");
            else
                _filters.Add(query);

            return this;
        }

        public IFilterUsable Build()
        {
            return this;
        }



        public string GetQuery()
        {
            return string.Join(" ", this._filters);
        }

        public IReadOnlyDictionary<string, string> GetNames()
        {
            return new ReadOnlyDictionary<string, string>(this._namesTokens);
        }

        public IReadOnlyDictionary<string, Tuple<object, Type, Type>> GetValues()
        {
            return new ReadOnlyDictionary<string, Tuple<object, Type, Type>>(this._valuesTokens);
        }



        private string BuildBinaryExpression(BinaryExpression body)
        {
            var left = body.Left;
            var right = body.Right;
            var operationType = body.NodeType;
            string leftValue = null;
            string rightValue = null;
            Tuple<string, PropertyType, Type> member;
            Tuple<object, Type> value;
            BinaryExpression binaryExpression;



            if (MetadataHelper.Instance.TryCastTo(left, out binaryExpression))
            {
                leftValue = BuildBinaryExpression(binaryExpression);
            }
            else if (left is MemberExpression)
            {
                member = GetMemberAccess(left as MemberExpression);
                leftValue = MapMemberAccess(member);
            }
            else if (left is ConstantExpression)
            {
                value = GetMemberValue(left as ConstantExpression);
            }






            if (MetadataHelper.Instance.TryCastTo(right, out binaryExpression))
                rightValue = BuildBinaryExpression(binaryExpression);


            
            else if(right is MemberExpression)
                member = GetMemberAccess(right as MemberExpression);
            else
                throw new Exception();



            
            else if (right is ConstantExpression)
                value = GetMemberValue(right as ConstantExpression);
            else
                throw new Exception();

            return GetBinayExpressionValue(left) + " " +
                   GetLogicalOperator(operationType) + " " +
                   GetBinayExpressionValue(right);
        }

        private string GetLogicalOperator(ExpressionType expressionType)
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

        private string GetLogicalOperator(ComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    return "=";
                case ComparisonType.Greater:
                    return ">";
                case ComparisonType.GreaterOrEqual:
                    return ">=";
                case ComparisonType.Less:
                    return "<";
                case ComparisonType.LessOrEqual:
                    return "<=";
                case ComparisonType.NotEqual:
                    return "<>";                
                default:
                    throw new NotSupportedException($"The omparison type '{comparisonType}' is not supported");
            }
        }

        private Tuple<string, PropertyType, Type> GetMemberAccess(MemberExpression expression)
        {
            var columnName = _propertyHelper.GetColumnName(expression);
            var columnType = _propertyHelper.GetColumnType(expression);
            var columnConverter = _propertyHelper.GetColumnConverter(expression);
            return new Tuple<string, PropertyType, Type>(columnName, columnType, columnConverter);
        }
        
        private Tuple<object, Type> GetMemberValue(ConstantExpression expression)
        {
            return new Tuple<object, Type>(expression.Value, expression.Type);
        }

        private string MapMemberAccess(Tuple<string, PropertyType, Type> member)
        {
            var alias = "#" + member.Item1;
            if(!_namesTokens.ContainsKey(alias))
                _namesTokens.Add(alias, member.Item1);
            return alias;
        }

        private string GetConcatenationValue(FilterConcatenationType concatenationType)
        {
            return concatenationType == FilterConcatenationType.And ? "AND" : "OR";
        }
        
    }
}
