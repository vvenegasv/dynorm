using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DynORM.Interfaces;

namespace DynORM.Implementations
{
    public class DynoResult<TModel> : IDynoResult<TModel> where TModel : class
    {
        private readonly IList<TModel> _data;
        private readonly int _consumedReadCapacity;
        private readonly int _consumedWrieCapacity;
        private readonly IDictionary<string, Tuple<object, Type>> _lastEvaluatedKey;

        public DynoResult(IList<TModel> data, int consumedReadCapacity, int consumedWrieCapacity, IDictionary<string, Tuple<object, Type>> lastEvaluatedKey)
        {
            _data = data;
            _consumedReadCapacity = consumedReadCapacity;
            _consumedWrieCapacity = consumedWrieCapacity;
            _lastEvaluatedKey = lastEvaluatedKey;
        }


        public int GetConsumedReadCapacity()
        {
            return _consumedReadCapacity;
        }

        public int GetConsumedWriteCapacity()
        {
            return _consumedWrieCapacity;
        }

        public IDictionary<string, Tuple<object, Type>> GetLasEvaluatedKey()
        {
            return _lastEvaluatedKey;
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            foreach (var item in _data)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
