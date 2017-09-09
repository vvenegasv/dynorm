using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynORM
{
    public interface IRepository<TModel> where TModel: class
    {
        Task<TModel> Create(TModel item);
        Task<TModel> Update(TModel item);
        Task<TModel> Delete(TModel item);
        Task<TModel> GetByPk<THashKey>(THashKey hashKey);
        Task<TModel> GetByPk<THashKey, TRangeKey>(THashKey hashKey, TRangeKey rangeKey);
        Task<IQueryable<TModel>> GetByHashKey<THashKey>(THashKey hashKey);
    }
}
