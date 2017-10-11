using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DynORM.Helpers;
using System.Collections.ObjectModel;

namespace DynORM.Filters
{
    public class DynamoDbFilterBuilder<TModel> : IFilterUsable, IFilterable<TModel> where TModel : class
    {
        private readonly IList<string> _filters;
        private readonly IDictionary<string, string> _namesTokens;
        private readonly IDictionary<string, KeyValuePair<object, Type>> _valuesTokens;
        private readonly PropertyHelper _propertyHelper;


        public DynamoDbFilterBuilder() : base()
        {
            _propertyHelper = PropertyHelper.Instance;
            _filters = new List<string>();
            _namesTokens = new Dictionary<string, string>();
            _valuesTokens = new Dictionary<string, KeyValuePair<object, Type>>();
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
                _valuesTokens.Add(param, new KeyValuePair<object, Type>(val, val.GetType()));
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
            _valuesTokens.Add(parameter, new KeyValuePair<object, Type>(substring, substring.GetType()));

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
            _valuesTokens.Add(parameter, new KeyValuePair<object, Type>(target, target.GetType()));

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
            _valuesTokens.Add(parameter, new KeyValuePair<object, Type>(value, value.GetType()));

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

        public IReadOnlyDictionary<string, KeyValuePair<object, Type>> GetValues()
        {
            return new ReadOnlyDictionary<string, KeyValuePair<object, Type>>(this._valuesTokens);
        }



        private string BuildBinaryExpression(BinaryExpression body)
        {
            var left = body.Left;
            var right = body.Right;
            var operationType = body.NodeType;
            
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

        private string GetBinayExpressionValue(Expression expression)
        {
            BinaryExpression binaryExpression;
            if (MetadataHelper.Instance.TryCastTo(expression, out binaryExpression))
                return BuildBinaryExpression(binaryExpression);

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var columnName = _propertyHelper.GetColumnName((MemberExpression)expression);
                var columnAlias = $"#{columnName}";
                if (!_namesTokens.ContainsKey(columnAlias))
                    _namesTokens.Add(columnAlias, columnName);
                return columnAlias;
            }

            if (expression.NodeType == ExpressionType.Constant)
            {
                var value = (string)(expression as ConstantExpression).Value;
                var index = _valuesTokens.Count + 1;
                var token = $":p{index}";
                _valuesTokens.Add(token, new KeyValuePair<object, Type>(value, typeof(string)));
                return token;
            }

            return string.Empty;
        }

        private string GetConcatenationValue(FilterConcatenationType concatenationType)
        {
            return concatenationType == FilterConcatenationType.And ? "AND" : "OR";
        }

        
    }
}
