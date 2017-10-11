using DynORM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DynORM
{
    public interface IRepository<TModel> where TModel: class
    {
        bool IsConsistentRead { get; set; }
        Task Create(TModel item);
        Task Create(TModel item, IFilterable<TModel> condition);
        Task Update(TModel item);
        Task Update(TModel item, IFilterable<TModel> condition);
        Task Update(dynamic item, IFilterable<TModel> condition);
        Task Delete(TModel item);
        Task Delete(TModel item, IFilterable<TModel> condition);
        Task Delete(IFilterable<TModel> condition);
        Task<TModel> GetOne<THashKey>(THashKey hashKey) where THashKey : class;
        Task<TModel> GetOne<THashKey, TRangeKey>(THashKey hashKey, TRangeKey rangeKey) where THashKey : class where TRangeKey : class;
        IScannable<TModel> Query<THashKey>(THashKey hashKey) where THashKey : class;
        IScannable<TModel> Query<THashKey>(THashKey hashKey, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey) where THashKey : class;
        IScannable<TProjection> Query<TIndex, TProjection>(IFilterable<TIndex> filter) where TIndex : class where TProjection : class;
        IScannable<TProjection> Query<TIndex, TProjection>(IFilterable<TIndex> filter, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey) where TIndex : class where TProjection : class;
        IScannable<TModel> Scan(IFilterable<TModel> filter);
        IScannable<TModel> Scan(IFilterable<TModel> filter, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey);
        IScannable<TModel> Scan();
        IScannable<TModel> Scan(Dictionary<string, Tuple<object, Type>> lastEvaluatedKey);
    }
}
