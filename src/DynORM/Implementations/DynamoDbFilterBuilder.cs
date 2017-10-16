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
        private readonly ItemHelper _itemHelper;
        private readonly MetadataHelper _metadataHelper;

        /// <summary>
        /// Dictionary Key=Parameter, Value=Tuple[Object Value, Destination Type, Convert Type]
        /// </summary>
        private readonly IDictionary<string, Tuple<object, PropertyType, Type>> _valuesTokens;
        private readonly IList<string> _filters;
        private readonly IDictionary<string, string> _namesTokens;


        public DynamoDbFilterBuilder() : base()
        {
            _itemHelper = ItemHelper.Instance;
            _metadataHelper = MetadataHelper.Instance;
            _filters = new List<string>();
            _namesTokens = new Dictionary<string, string>();
            _valuesTokens = new Dictionary<string, Tuple<object, PropertyType, Type>>();
        }

        public DynamoDbFilterBuilder(string filter)
        {
            _itemHelper = ItemHelper.Instance;
            _metadataHelper = MetadataHelper.Instance;
            _filters = new List<string>();
            _filters.Add(filter);
            _namesTokens = new Dictionary<string, string>();
            _valuesTokens = new Dictionary<string, Tuple<object, PropertyType, Type>>();
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

        public IFilterable<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values)
        {
            return WhereIn(property, values, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values, FilterConcatenationType concatenationType)
        {
            if (property?.NodeType != ExpressionType.Lambda)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");
                        
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            string filter = string.Empty;
            if (_filters.Any())
                filter = GetConcatenationValue(concatenationType);
            var columnName = _itemHelper.GetColumnName(memberExpression);
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
                _valuesTokens.Add(param, new Tuple<object, PropertyType, Type>(val, _itemHelper.GetColumnType(memberExpression), _itemHelper.GetColumnConverter(memberExpression)));
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

            var name = _itemHelper.GetColumnName(memberExpression);
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

            var name = _itemHelper.GetColumnName(memberExpression);
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

        public IFilterable<TModel> WhereBeginsWith<TValue>(Expression<Func<TModel, TValue>> property, string substring)
        {
            return WhereBeginsWith(property, substring, FilterConcatenationType.And);
        }

        public IFilterable<TModel> WhereBeginsWith<TValue>(Expression<Func<TModel, TValue>> property, string substring, FilterConcatenationType concatenationType)
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ExpressionNotSupportedException($"Expression {property} is unsupported");

            var name = _itemHelper.GetColumnName(memberExpression);
            var key = "#" + name;
            if (!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, name);

            var index = _valuesTokens.Count + 1;
            var parameter = ":p" + index;
            _valuesTokens.Add(parameter, new Tuple<object, PropertyType, Type>(substring, PropertyType.String, null));

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

            var name = _itemHelper.GetColumnName(memberExpression);
            var key = "#" + name;
            if (!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, name);

            var index = _valuesTokens.Count + 1;
            var parameter = ":p" + index;
            _valuesTokens.Add(parameter, new Tuple<object, PropertyType, Type>(target, GetPropertyType(target.GetType()), null));

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

            var name = _itemHelper.GetColumnName(memberExpression);
            var key = "#" + name;
            if (!_namesTokens.ContainsKey(key))
                _namesTokens.Add(key, name);

            var index = _valuesTokens.Count + 1;
            var parameter = ":p" + index;
            _valuesTokens.Add(parameter, new Tuple<object, PropertyType, Type>(value, GetPropertyType(value.GetType()), null));

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

        public IReadOnlyDictionary<string, Tuple<object, PropertyType, Type>> GetValues()
        {
            return new ReadOnlyDictionary<string, Tuple<object, PropertyType, Type>>(this._valuesTokens);
        }


        /// <summary>
        /// Get a dynamodb query string for the given expression
        /// </summary>
        /// <param name="body">Expression that refers to a boolean operation</param>
        /// <returns>Dynamodb query string for the given expression</returns>
        private string BuildBinaryExpression(BinaryExpression body)
        {
            var left = body.Left;
            var right = body.Right;
            var operationType = body.NodeType;
            string leftStringExpression = null;
            string rightStringExpression = null;
            Tuple<string, PropertyType, Type> leftMember = null;
            Tuple<string, PropertyType, Type> rightMember = null;
            object leftValue = null;
            object rightValue = null;
            BinaryExpression binaryExpression;
            
            //Get left expression
            if (MetadataHelper.Instance.TryCastTo(left, out binaryExpression))
            {
                leftStringExpression = BuildBinaryExpression(binaryExpression);
            }
            else if (left is MemberExpression)
            {
                var memberExpression = left as MemberExpression;
                if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
                {
                    leftMember = GetMemberInfo(memberExpression);
                    leftStringExpression = MapMemberInfo(leftMember);
                }
                else if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                {
                    leftValue = GetMemberValue(memberExpression);
                }
                else
                {
                    throw new ExpressionNotSupportedException($"The expression '{memberExpression}' cannot be resolved");
                }
            }
            else if (left is ConstantExpression)
            {
                leftValue = GetMemberValue(left as ConstantExpression);
            }
            else
            {
                throw new ExpressionNotSupportedException($"Left expression {left}, is not supported. It need to be one of these: MemberExpression or ConstantExpression.");
            }

            
            //Get right expression
            if (MetadataHelper.Instance.TryCastTo(right, out binaryExpression))
            {
                rightStringExpression = BuildBinaryExpression(binaryExpression);
            }
            else if (right is MemberExpression)
            {
                var memberExpression = right as MemberExpression;
                if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
                {
                    rightMember = GetMemberInfo(memberExpression);
                    rightStringExpression = MapMemberInfo(leftMember);
                }
                else if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                {
                    rightValue = GetMemberValue(memberExpression);
                }
                else
                {
                    throw new ExpressionNotSupportedException($"The expression '{memberExpression}' cannot be resolved");
                }
            }
            else if (right is ConstantExpression)
            {
                rightValue = GetMemberValue(right as ConstantExpression);
            }
            else
            {
                throw new ExpressionNotSupportedException($"Right expression {right}, is not supported. It need to be one of these: MemberExpression or ConstantExpression.");
            }


            // Get converter from left or right and make the value item
            if (leftValue != null || rightValue != null)
            {
                if (leftValue == null)
                {
                    rightStringExpression = MapMemberValue(rightValue, leftMember.Item2, leftMember.Item3);
                }
                else if (rightValue == null)
                {
                    leftStringExpression = MapMemberValue(leftValue, rightMember.Item2, rightMember.Item3);
                }
                else
                {
                    throw new ExpressionNotSupportedException($"The expression {body} has only constant. It need to has at least one MemberExpression");
                }
            }

            if (body.NodeType == ExpressionType.AndAlso || body.NodeType == ExpressionType.OrElse)
            {
                if(leftStringExpression.Contains(" OR ") || leftStringExpression.Contains(" AND "))
                    leftStringExpression = $"({leftStringExpression})";
                if (rightStringExpression.Contains(" OR ") || rightStringExpression.Contains(" AND "))
                    rightStringExpression = $"({rightStringExpression})";
            }
            
            return leftStringExpression + " " +
                   GetLogicalOperator(operationType) + " " +
                   rightStringExpression;
        }

        /// <summary>
        /// Get a logical operator string for the given expression type
        /// </summary>
        /// <param name="expressionType">Expression logical operator</param>
        /// <returns>String for the given expression type</returns>
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
                case ExpressionType.OrElse:
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

        /// <summary>
        /// Get a logical operator string for the given comparison type
        /// </summary>
        /// <param name="comparisonType">Logical operator</param>
        /// <returns>string for the given comparison type</returns>
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

        /// <summary>
        /// Get the member information as Tuple --> Item1=ColumnName, Item2=ColumnType, Item3=ColumnConverter
        /// </summary>
        /// <param name="expression">Expression that point to a property</param>
        /// <returns>Tuple Item1=ColumnName, Item2=ColumnType, Item3=ColumnConverter</returns>
        private Tuple<string, PropertyType, Type> GetMemberInfo(MemberExpression expression)
        {
            var columnName = _itemHelper.GetColumnName(expression);
            var columnType = _itemHelper.GetColumnType(expression);
            var columnConverter = _itemHelper.GetColumnConverter(expression);
            return new Tuple<string, PropertyType, Type>(columnName, columnType, columnConverter);
        }
        
        /// <summary>
        /// Get the expression value
        /// </summary>
        /// <param name="expression">ConstantExpression with object value</param>
        /// <returns>Object value</returns>
        private object GetMemberValue(ConstantExpression expression)
        {
            return expression.Value;
        }

        /// <summary>
        /// Get the expression value
        /// </summary>
        /// <param name="expression">MemberExpression with object value</param>
        /// <returns>Object value</returns>
        private object GetMemberValue(MemberExpression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        /// <summary>
        /// Get alias for a member information as Tuple
        /// </summary>
        /// <param name="member">Tuple Item1=ColumnName, Item2=PropertyType, Item3=ConverterType</param>
        /// <returns>Alias for current member</returns>
        private string MapMemberInfo(Tuple<string, PropertyType, Type> member)
        {
            var alias = "#" + member.Item1;
            if(!_namesTokens.ContainsKey(alias))
                _namesTokens.Add(alias, member.Item1);
            return alias;
        }

        /// <summary>
        /// Get parameter for a member value
        /// </summary>
        /// <param name="value">Member value</param>
        /// <param name="propertyType">Property type that is access</param>
        /// <param name="converter">Converter</param>
        /// <returns></returns>
        private string MapMemberValue(object value, PropertyType propertyType, Type converter)
        {
            var index = _valuesTokens.Count + 1;
            var key = ":p" + index;
            _valuesTokens.Add(key, new Tuple<object, PropertyType, Type>(value, propertyType, converter));
            return key;
        }

        /// <summary>
        /// Gets the logical concatenador string 
        /// </summary>
        /// <param name="concatenationType">Logical concatenador</param>
        /// <returns>String for the current logical concatenador</returns>
        private string GetConcatenationValue(FilterConcatenationType concatenationType)
        {
            return concatenationType == FilterConcatenationType.And ? "AND" : "OR";
        }

        /// <summary>
        /// Gets the DynamoDB type for the current property type
        /// </summary>
        /// <param name="type">Property type</param>
        /// <returns>DynamoDB property type</returns>
        private PropertyType GetPropertyType(Type type)
        {
            if(_metadataHelper.IsNumber(type))
                return PropertyType.Number;
            if(_metadataHelper.IsBoolean(type))
                return PropertyType.Boolean;
            return PropertyType.String;
        }

    }
}
