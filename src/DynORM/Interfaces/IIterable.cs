using System;
using System.Collections.Generic;
using System.Text;

namespace DynORM.Filters
{
    public interface IIterable<TModel>: IEnumerable<TModel> where TModel : class
    {
        IDictionary<string, Tuple<object, Type>> GetLasEvaluatedKey();
        int GetConsumedReadCapacity();
        int GetConsumedWriteCapacity();
    }
}
