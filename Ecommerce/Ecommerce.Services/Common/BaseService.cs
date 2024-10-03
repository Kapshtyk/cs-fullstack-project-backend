using Ecommerce.Domain.Common;
using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Services.Common.DTO;
using Ecommerce.Services.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Services.Common
{
    public partial class BaseService<T, TFilter, TReadDto, TCreateDto, TUpdateDto, TPartialUpdateDto>(
            IBaseRepo<T, TFilter> repo,
            IDistributedCache cache
            ) :
            IBaseService<T, TFilter, TReadDto, TPartialUpdateDto, TCreateDto, TUpdateDto>
                where T : BaseEntity
                where TFilter : PaginationOptionsBase<T>
                where TReadDto : IReadDto<T>
                where TCreateDto : ICreateDto<T>
                where TUpdateDto : IUpdateDto<T>
                where TPartialUpdateDto : IPartialUpdateDto<T>
    {
        private readonly IBaseRepo<T, TFilter> _repo = repo;
        private readonly IDistributedCache _cache = cache;

        public virtual async Task<TReadDto> CreateAsync(TCreateDto createDto)
        {
            var entity = await _repo.CreateAsync(createDto.GetEntity());
            var readDto = Activator.CreateInstance<TReadDto>();
            readDto.FromEntity(entity);
            await AddValueToCache(readDto);

            return readDto;
        }

        public virtual async Task<PaginatedResult<T, TReadDto>> GetAllAsync(TFilter filteringOptions)
        {
            var total = await _repo.CountAsync(filteringOptions);
            var query = await _repo.GetAllAsync(filteringOptions);
            var result = new List<TReadDto>();
            foreach (var entity in query)
            {
                var dto = Activator.CreateInstance<TReadDto>();

                dto.FromEntity(entity);
                result.Add(dto);
            }
            var paginatedResult = new PaginatedResult<T, TReadDto>
            {
                ItemsPerPage = filteringOptions.PerPage,
                CurrentPage = filteringOptions.Page,
                TotalItems = total,
                Items = result
            };

            return paginatedResult;
        }

        public virtual async Task<TReadDto> GetByIdAsync(int id)
        {
            GenerateCacheKey(out string key, typeof(T).Name, id);
            try
            {
                var cachedResult = await GetEntityFromCache(key);
                if (!EqualityComparer<TReadDto>.Default.Equals(cachedResult, default(TReadDto)))
                {
                    return cachedResult;
                }
            }
            catch (EntityNotFoundException<T>)
            {
                Console.WriteLine($"Entity with id {id} was not found in cache error 1");
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine($"Entity with id {id} was not found in cache error 2");
                // We don't want to throw an exception if the cache is not available or something else goes wrong with deserialization
            }

            var query = await _repo.GetByIdAsync(id);
            var dto = Activator.CreateInstance<TReadDto>();
            dto.FromEntity(query);
            await AddValueToCache(dto);

            return dto;
        }

        public virtual async Task<TReadDto> UpdateAsync(TUpdateDto updateDto, int id)
        {
            var entity = updateDto.GetUpdatedEntity(await _repo.GetByIdAsync(id));

            var result = await _repo.UpdateAsync(entity);

            var dto = Activator.CreateInstance<TReadDto>();
            dto.FromEntity(result);
            await AddValueToCache(dto);

            return dto;
        }

        public virtual async Task<TReadDto> UpdateAsync(TPartialUpdateDto updateDto, int id)
        {
            var entity = updateDto.GetUpdatedEntity(await _repo.GetByIdAsync(id));

            var result = await _repo.UpdateAsync(entity);

            var dto = Activator.CreateInstance<TReadDto>();
            dto.FromEntity(result);
            await AddValueToCache(dto);

            return dto;
        }

        public virtual async Task<bool> DeleteByIdAsync(int id)
        {
            var result = await _repo.DeleteByIdAsync(id);
            if (result)
            {
                await MarkAsDeleted(id);
            }
            return result;
        }
    }
}