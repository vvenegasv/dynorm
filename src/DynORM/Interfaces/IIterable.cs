﻿using System;
using System.Collections.Generic;

namespace DynORM.Interfaces
{
    public interface IIterable<TModel>: IEnumerable<TModel> where TModel : class
    {
        IDictionary<string, Tuple<object, Type>> GetLasEvaluatedKey();
        int GetConsumedReadCapacity();
        int GetConsumedWriteCapacity();
    }
}
