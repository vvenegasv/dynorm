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
        Task Create(TModel item);
        Task Create(TModel item, IFilterBuilder<TModel> condition);
        Task Update(TModel item);
        Task Update(TModel item, IFilterBuilder<TModel> condition);
        Task Update(dynamic item, IFilterBuilder<TModel> condition);
        Task Delete(TModel item);
        Task Delete(TModel item, IFilterBuilder<TModel> condition);
        Task Delete(IFilterBuilder<TModel> condition);
        Task<TModel> GetOne<THashKey>(THashKey hashKey);
        Task<TModel> GetOne<THashKey, TRangeKey>(THashKey hashKey, TRangeKey rangeKey);
        Task<IEnumerable<TModel>> QueryByIndex<TIndex>(IFilterBuilder<TIndex> filter) where TIndex : class;
    }
}
