using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DynORM.Helpers;

namespace DynORM.Filters
{
    public class DynamoDbFilterBuilder<TModel> : IFilterBuilder<TModel> where TModel : class
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


        public IFilterBuilder<TModel> Where(Expression<Func<TModel, bool>> expression)
        {
            return Where(expression, FilterConcatenationType.And);
        }

        public IFilterBuilder<TModel> Where(Expression<Func<TModel, bool>> expression, FilterConcatenationType concatenationType)
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

        public IFilterBuilder<TModel> Where(IFilterBuilder<TModel> filter)
        {
            return Where(filter, FilterConcatenationType.And);
        }

        public IFilterBuilder<TModel> Where(IFilterBuilder<TModel> filter, FilterConcatenationType concatenationType)
        {
            if(filter == null)
                throw new ArgumentNullException(nameof(filter), $"{nameof(filter)} cannot be null");

            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} ({filter.Build()})");
            else
                _filters.Add($"({filter.Build()})");

            return this;
        }

        public IFilterBuilder<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values) where TValue : class
        {
            return WhereIn(property, values, FilterConcatenationType.And);
        }

        public IFilterBuilder<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values, FilterConcatenationType concatenationType) where TValue : class
        {
            if (property?.NodeType != ExpressionType.Lambda)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");
                        
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} {_propertyHelper.GetColumnName(memberExpression)} in ({string.Join(", ", values)})");
            else
                _filters.Add($"{_propertyHelper.GetColumnName(memberExpression)} in ({string.Join(", ", values)})");
            
            return this;
        }

        public IFilterBuilder<TModel> WhereAttributeExists<TValue>(Expression<Func<TModel, TValue>> property) where TValue : class
        {
            return WhereAttributeExists(property, FilterConcatenationType.And);
        }

        public IFilterBuilder<TModel> WhereAttributeExists<TValue>(Expression<Func<TModel, TValue>> property, FilterConcatenationType concatenationType) where TValue : class
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");


            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} attribute_exists ({_propertyHelper.GetColumnName(memberExpression)})");
            else
                _filters.Add($"attribute_exists ({_propertyHelper.GetColumnName(memberExpression)})");

            return this;
        }

        public IFilterBuilder<TModel> WhereAttributeExists(string property)
        {
            return WhereAttributeExists(property, FilterConcatenationType.And);
        }

        public IFilterBuilder<TModel> WhereAttributeExists(string property, FilterConcatenationType concatenationType)
        {
            if (_filters.Any())
                _filters.Add($"{GetConcatenationValue(concatenationType)} attribute_exists ({property})");
            else
                _filters.Add($"attribute_exists ({property})");

            return this;
        }

        public IFilterBuilder<TModel> WhereAttributeNotExists<TValue>(Expression<Func<TModel, TValue>> property) where TValue : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereAttributeNotExists<TValue>(Expression<Func<TModel, TValue>> property, FilterConcatenationType concatenationType) where TValue : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereAttributeNotExists(string property)
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereAttributeNotExists(string property, FilterConcatenationType concatenationType)
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereBeginsWith<TValue>(Expression<Func<TModel, TValue>> property) where TValue : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereBeginsWith<TValue>(Expression<Func<TModel, TValue>> property, FilterConcatenationType concatenationType) where TValue : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereContains<TValue>(Expression<Func<TModel, TValue>> property, string target) where TValue : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereContains<TValue>(Expression<Func<TModel, TValue>> property, string target, FilterConcatenationType concatenationType) where TValue : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereContains<TValue, TTarget>(Expression<Func<TModel, TValue>> property, TTarget target) where TValue : class where TTarget : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereContains<TValue, TTarget>(Expression<Func<TModel, TValue>> property, TTarget target,
            FilterConcatenationType concatenationType) where TValue : class where TTarget : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereSize<TValue>(Expression<Func<TModel, TValue>> property, ComparisonType comparisonType, int value) where TValue : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereSize<TValue>(Expression<Func<TModel, TValue>> property, ComparisonType comparisonType, int value,
            FilterConcatenationType concatenationType) where TValue : class
        {
            throw new NotImplementedException();
        }

        public string Build()
        {
            return string.Join(" ", _filters);
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
