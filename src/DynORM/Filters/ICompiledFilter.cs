using System;
using System.Collections.Generic;
using System.Text;

namespace DynORM.Filters
{
    public interface ICompiledFilter
    {
        /// <summary>
        /// Get the compiled query string
        /// </summary>
        /// <returns>Query string</returns>
        string GetQuery();

        /// <summary>
        /// Get columns names and alias
        /// </summary>
        /// <returns>Dictionary with alias and name</returns>
        IReadOnlyDictionary<string, string> GetNames();

        /// <summary>
        /// Get parameter values
        /// </summary>
        /// <returns>Dictionary with parameter values</returns>
        IReadOnlyDictionary<string, KeyValuePair<object, Type>> GetValues();
    }
}
