using Ecommerce.Domain.Common;
using Ecommerce.Domain.Filters;
using Ecommerce.Services.Common;
using Ecommerce.Services.Common.DTO;
using Ecommerce.Services.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]s")]
    public class AppController<T, TFilter, TReadDto, TPartialUpdateDto, TCreateDto, TUpdateDto>(
        IBaseService<T, TFilter, TReadDto, TPartialUpdateDto, TCreateDto, TUpdateDto> baseService
        ) :
        ControllerBase
            where T : BaseEntity
            where TFilter : PaginationOptionsBase<T>
            where TReadDto : IReadDto<T>
            where TPartialUpdateDto : IPartialUpdateDto<T>
            where TCreateDto : ICreateDto<T>
            where TUpdateDto : IUpdateDto<T>
    {
        private readonly IBaseService<T, TFilter, TReadDto, TPartialUpdateDto, TCreateDto, TUpdateDto> _baseService = baseService;

        [HttpGet]
        public virtual async Task<ActionResult<PaginatedResult<T, TReadDto>>> GetItems([FromQuery] TFilter filteringOptions)
        {
            var items = await _baseService.GetAllAsync(filteringOptions);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TReadDto>> GetItem(int id)
        {
            var item = await _baseService.GetByIdAsync(id);

            return Ok(item);
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<T>> DeleteItem(int id)
        {
            await _baseService.DeleteByIdAsync(id);

            return NoContent();
        }

        [HttpPost]
        public virtual async Task<ActionResult<TReadDto>> CreateItem([FromBody] TCreateDto createDto)
        {
            var item = await _baseService.CreateAsync(createDto);

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TReadDto>> UpdateItem(int id, [FromBody] TUpdateDto updateDto)
        {
            var item = await _baseService.UpdateAsync(updateDto, id);

            return Ok(item);
        }

        [HttpPatch("{id}")]
        public virtual async Task<ActionResult<TReadDto>> PartialUpdateItem(int id, [FromBody] TPartialUpdateDto partialUpdateDto)
        {
            var item = await _baseService.UpdateAsync(partialUpdateDto, id);

            return Ok(item);
        }
    }
};