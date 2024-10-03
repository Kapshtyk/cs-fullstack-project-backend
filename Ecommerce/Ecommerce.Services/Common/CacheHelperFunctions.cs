using System.Text;
using System.Text.Json;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Services.Common.DTO;
using Ecommerce.Services.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Services.Common
{
    public partial class BaseService<T, TFilter, TReadDto, TCreateDto, TUpdateDto, TPartialUpdateDto> :
            IBaseService<T, TFilter, TReadDto, TPartialUpdateDto, TCreateDto, TUpdateDto>
                where T : BaseEntity
                where TFilter : PaginationOptionsBase<T>
                where TReadDto : IReadDto<T>
                where TCreateDto : ICreateDto<T>
                where TUpdateDto : IUpdateDto<T>
                where TPartialUpdateDto : IPartialUpdateDto<T>
    {
        protected string GenerateCacheKey(out string key, params object[] keyParts)
        {
            var keyBuilder = new StringBuilder();

            foreach (var part in keyParts)
            {
                keyBuilder.Append(':').Append(part);
            }

            key = keyBuilder.ToString();

            return key;
        }

        protected async Task AddValueToCache(TReadDto entity)
        {
            GenerateCacheKey(out string key, typeof(T).Name, entity.Id);
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(entity), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        protected async Task MarkAsDeleted(int id)
        {
            GenerateCacheKey(out string key, typeof(T).Name, id);
            await _cache.SetStringAsync(key, "DELETED", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });
        }

        protected async Task<TReadDto?> GetEntityFromCache(string key)
        {
            var cachedValue = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cachedValue))
            {
                if (cachedValue == "DELETED") throw new EntityNotFoundException<T>();

                var entity = JsonSerializer.Deserialize<TReadDto>(cachedValue);
                if (!EqualityComparer<TReadDto>.Default.Equals(entity, default(TReadDto)))
                {
                    return entity;
                }
            }
            return default;
        }
    }
}