using Ecommerce.Domain.Common;
using Ecommerce.Domain.Filters;

namespace Ecommerce.Domain.Interfaces
{
    public interface IBaseRepo<T, TFilter>
        where T : BaseEntity
        where TFilter : PaginationOptionsBase<T>
    {
        Task<T> CreateAsync(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(TFilter filteringOptions);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteByIdAsync(int id);
        Task<int> CountAsync(TFilter filteringOptions);
    }
}