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
        Repository<TModel> AddConditiion(Expression<Func<TModel, bool>> filter);

        Task Create(TModel item);
        Task Update(TModel item);
        Task Delete(TModel item);
        Task<TModel> GetByPk<THashKey>(THashKey hashKey);
        Task<TModel> GetByPk<THashKey, TRangeKey>(THashKey hashKey, TRangeKey rangeKey);
        Task<IQueryable<TModel>> GetByHashKey<THashKey>(THashKey hashKey);
    }
}
