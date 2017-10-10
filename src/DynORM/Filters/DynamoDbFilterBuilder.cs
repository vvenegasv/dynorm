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


        public DynamoDbFilterBuilder() : base()
        {
            _filters = new List<string>();
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
                _filters.Add($"{GetConcatenationValue(concatenationType)} {filter.Build()}");
            else
                _filters.Add(filter.Build());

            return this;
        }

        public IFilterBuilder<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values) where TValue : class
        {
            return WhereIn(property, values, FilterConcatenationType.And);
        }

        public IFilterBuilder<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values, FilterConcatenationType concatenationType) where TValue : class
        {
            if (property?.NodeType != ExpressionType.MemberAccess)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            Expression a = property;
            MemberExpression b = (MemberExpression)((Expression)property);
            var c = b.Member.Name;
            return this;
        }

        public IFilterBuilder<TModel> WhereAttributeExists<TValue>(Expression<Func<TModel, TValue>> property) where TValue : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereAttributeExists<TValue>(Expression<Func<TModel, TValue>> property, FilterConcatenationType concatenationType) where TValue : class
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereAttributeExists(string property)
        {
            throw new NotImplementedException();
        }

        public IFilterBuilder<TModel> WhereAttributeExists(string property, FilterConcatenationType concatenationType)
        {
            throw new NotImplementedException();
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

            return GetValue(left) + " " +
                   GetLogicalOperator(operationType) + " " +
                   GetValue(right);
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

        private string GetValue(Expression expression)
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

        private string GetConcatenationValue(FilterConcatenationType concatenationType)
        {
            return concatenationType == FilterConcatenationType.And ? "AND" : "OR";
        }

        
    }
}
