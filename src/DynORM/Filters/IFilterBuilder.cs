using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DynORM.Filters
{
    public interface IFilterBuilder<TModel> where TModel : class
    {
        /// <summary>
        /// True if the input expression returns true
        /// </summary>
        /// <param name="expression">Expression to evaluate. It only support basic comparison like a=b. Functions with contains and other stuff is not supported</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> Where(Expression<Func<TModel, bool>> expression);

        /// <summary>
        /// True if the input expression returns true
        /// </summary>
        /// <param name="expression">Expression to evaluate. It only support basic comparison like a=b. Functions with contains and other stuff is not supported</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> Where(Expression<Func<TModel, bool>> expression, FilterConcatenationType concatenationType);

        /// <summary>
        /// True if the input expression returns true
        /// </summary>
        /// <param name="filter">FilterBuilder instance</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> Where(IFilterBuilder<TModel> filter);

        /// <summary>
        /// True if the input expression returns true
        /// </summary>
        /// <param name="filter">FilterBuilder instance</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> Where(IFilterBuilder<TModel> filter, FilterConcatenationType concatenationType);



        /// <summary>
        /// True if the property value is equal to any value in the list
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute</param>
        /// <param name="values">Value list</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values) where TValue : class;

        /// <summary>
        /// True if the property value is equal to any value in the list
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute</param>
        /// <param name="values">Value list</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereIn<TValue>(Expression<Func<TModel, TValue>> property, IEnumerable<TValue> values, FilterConcatenationType concatenationType) where TValue : class;



        /// <summary>
        /// True if contains the specified attribute
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereAttributeExists<TValue>(Expression<Func<TModel, TValue>> property) where TValue : class;

        /// <summary>
        /// True if contains the specified attribute
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereAttributeExists<TValue>(Expression<Func<TModel, TValue>> property, FilterConcatenationType concatenationType) where TValue : class;

        /// <summary>
        /// True if contains the specified attribute
        /// </summary>
        /// <param name="property">Attribute name</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereAttributeExists(string property);

        /// <summary>
        /// True if contains the specified attribute
        /// </summary>
        /// <param name="property">Attribute name</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereAttributeExists(string property, FilterConcatenationType concatenationType);



        /// <summary>
        /// True if the specified attribute dos not exists
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereAttributeNotExists<TValue>(Expression<Func<TModel, TValue>> property) where TValue : class;

        /// <summary>
        /// True if the specified attribute dos not exists
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereAttributeNotExists<TValue>(Expression<Func<TModel, TValue>> property, FilterConcatenationType concatenationType) where TValue : class;

        /// <summary>
        /// True if the specified attribute dos not exists
        /// </summary>
        /// <param name="property">Attribute name</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereAttributeNotExists(string property);

        /// <summary>
        /// True if the specified attribute dos not exists
        /// </summary>
        /// <param name="property">Attribute name</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereAttributeNotExists(string property, FilterConcatenationType concatenationType);



        /// <summary>
        /// True if the attribute specified begins with a particular substring
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute specified value</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereBeginsWith<TValue>(Expression<Func<TModel, TValue>> property) where TValue : class;

        /// <summary>
        /// True if the attribute specified begins with a particular substring
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute specified value</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereBeginsWith<TValue>(Expression<Func<TModel, TValue>> property, FilterConcatenationType concatenationType) where TValue : class;



        /// <summary>
        /// True if the attribute specified contains a particular substring
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute specified value</param>
        /// <param name="target">Substring to looking for</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereContains<TValue>(Expression<Func<TModel, TValue>> property, string target) where TValue : class;

        /// <summary>
        /// True if the attribute specified contains a particular substring
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute specified value</param>
        /// <param name="target">Substring to looking for</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereContains<TValue>(Expression<Func<TModel, TValue>> property, string target, FilterConcatenationType concatenationType) where TValue : class;

        /// <summary>
        /// True if the attribute specified contains a element within
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <typeparam name="TTarget">Object to looking for</typeparam>
        /// <param name="property">Attribute specified value</param>
        /// <param name="target">Element to looking for</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereContains<TValue, TTarget>(Expression<Func<TModel, TValue>> property, TTarget target)
            where TValue : class
            where TTarget : class;

        /// <summary>
        /// True if the attribute specified contains a element within
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <typeparam name="TTarget">Object to looking for</typeparam>
        /// <param name="property">Attribute specified value</param>
        /// <param name="target">Element to looking for</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereContains<TValue, TTarget>(Expression<Func<TModel, TValue>> property, TTarget target, FilterConcatenationType concatenationType)
            where TValue : class
            where TTarget : class;



        /// <summary>
        /// Returns true if size satisfied the comparison type.
        /// For string = lenght
        /// For arrays = number of elements
        /// For binary = binary size
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute to get size</param>
        /// <param name="comparisonType">Comparison type. Note that it must be read left to right</param>
        /// <param name="value">Value to check</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereSize<TValue>(Expression<Func<TModel, TValue>> property, ComparisonType comparisonType, int value) where TValue : class;

        /// <summary>
        /// Returns true if size satisfied the comparison type.
        /// For string = lenght
        /// For arrays = number of elements
        /// For binary = binary size
        /// </summary>
        /// <typeparam name="TValue">Attribute specified type</typeparam>
        /// <param name="property">Attribute to get size</param>
        /// <param name="comparisonType">Comparison type. Note that it must be read left to right</param>
        /// <param name="value">Value to check</param>
        /// <param name="concatenationType">Concatenation type (And/Or)</param>
        /// <returns>Updated filter builder</returns>
        IFilterBuilder<TModel> WhereSize<TValue>(Expression<Func<TModel, TValue>> property, ComparisonType comparisonType, int value, FilterConcatenationType concatenationType) where TValue : class;

        /// <summary>
        /// Create the query 
        /// </summary>
        /// <returns>string that represent filters given</returns>
        string Build();
    }
}
