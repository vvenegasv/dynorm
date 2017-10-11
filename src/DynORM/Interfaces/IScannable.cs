using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DynORM.Filters
{
    public interface IScannable<TModel> where TModel : class
    {
        IScannable<TModel> Filter(IFilterable<TModel> filter);
        IScannable<TModel> Take(int amount);
        IScannable<TModel> OrderBy();
        IScannable<TModel> OrderByDescending();
        Task<IIterable<TModel>> MakeScan();
    }
}
