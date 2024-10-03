using Ecommerce.Domain.Common;
using Ecommerce.Domain.Filters;
using Ecommerce.Services.Common.DTO;

namespace Ecommerce.Services.Common.Interfaces
{
    public interface IBaseService<T, TFilter, TReadDto, TPartialUpdateDto, TCreateDto, TUpdateDto>
        where T : BaseEntity
        where TFilter : PaginationOptionsBase<T>
        where TCreateDto : ICreateDto<T>
        where TReadDto : IReadDto<T>
        where TUpdateDto : IUpdateDto<T>
        where TPartialUpdateDto : IPartialUpdateDto<T>
    {
        Task<TReadDto> CreateAsync(TCreateDto entity);
        Task<TReadDto> UpdateAsync(TUpdateDto entity, int id);
        Task<TReadDto> UpdateAsync(TPartialUpdateDto entity, int id);
        Task<PaginatedResult<T, TReadDto>> GetAllAsync(TFilter filteringOptions);
        Task<TReadDto> GetByIdAsync(int id);
        Task<bool> DeleteByIdAsync(int id);
    }
}