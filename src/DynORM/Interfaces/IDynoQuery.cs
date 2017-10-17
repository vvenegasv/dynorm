using System.Threading.Tasks;

namespace DynORM.Interfaces
{
    public interface IDynoQuery<TModel> where TModel : class
    {
        IDynoQuery<TModel> Filter(IDynoFilter<TModel> filter);
        IDynoQuery<TModel> Take(int amount);
        IDynoQuery<TModel> OrderBy();
        IDynoQuery<TModel> OrderByDescending();
        Task<IDynoResult<TModel>> MakeScan();
    }
}
