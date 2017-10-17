using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynORM.Interfaces
{
    public interface IDynoRepo<TModel> where TModel: class
    {
        bool IsConsistentRead { get; set; }
        Task Create(TModel item);
        Task Create(TModel item, IDynoFilter<TModel> condition);
        Task Update(TModel item);
        Task Update(TModel item, IDynoFilter<TModel> condition);
        Task Update(dynamic item, IDynoFilter<TModel> condition);
        Task Delete(TModel item);
        Task Delete(TModel item, IDynoFilter<TModel> condition);
        Task Delete(IDynoFilter<TModel> condition);
        Task<TModel> GetOne<THashKey>(THashKey hashKey) where THashKey : class;
        Task<TModel> GetOne<THashKey, TRangeKey>(THashKey hashKey, TRangeKey rangeKey) where THashKey : class where TRangeKey : class;
        IDynoQuery<TModel> Query<THashKey>(THashKey hashKey) where THashKey : class;
        IDynoQuery<TModel> Query<THashKey>(THashKey hashKey, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey) where THashKey : class;
        IDynoQuery<TProjection> Query<TIndex, TProjection>(IDynoFilter<TIndex> filter) where TIndex : class where TProjection : class;
        IDynoQuery<TProjection> Query<TIndex, TProjection>(IDynoFilter<TIndex> filter, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey) where TIndex : class where TProjection : class;
        IDynoQuery<TModel> Scan(IDynoFilter<TModel> filter);
        IDynoQuery<TModel> Scan(IDynoFilter<TModel> filter, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey);
        IDynoQuery<TModel> Scan();
        IDynoQuery<TModel> Scan(Dictionary<string, Tuple<object, Type>> lastEvaluatedKey);
    }
}
