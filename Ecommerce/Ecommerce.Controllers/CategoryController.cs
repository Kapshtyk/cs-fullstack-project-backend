using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Models;
using Ecommerce.Services.CategoryService.DTO;
using Ecommerce.Services.CategoryService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FluentValidation.Results;
using Ecommerce.Services.Common;
using Microsoft.AspNetCore.Authorization;
using static Ecommerce.Services.CategoryService.DTO.CreateCategoryDto;
using static Ecommerce.Services.CategoryService.DTO.UpdateCategoryDto;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/v1/categories")]
    [SwaggerTag("Create, read, update and delete categories")]
    public class CategoryController(ICategoryService categoryService) : AppController<Category, CategoryFilterOptions, GetCategoryDto, PartialUpdateCategoryDto, CreateCategoryDto, UpdateCategoryDto>(categoryService)
    {
        private readonly ICategoryService _categoryService = categoryService;
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get a list of categories", Description = "Permission: All users")]
        [SwaggerResponse(200, "The categories were fetched successfully", typeof(PaginatedResult<Category, GetCategoryDto>))]
        [SwaggerResponse(401, "Unauthorized")]
        async public override Task<ActionResult<PaginatedResult<Category, GetCategoryDto>>> GetItems([FromQuery] CategoryFilterOptions filteringOptions)
        {
            return await base.GetItems(filteringOptions);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get a category by id", Description = "Permission: All users")]
        [SwaggerResponse(200, "The category was updated successfully", typeof(GetCategoryDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "The category was not found", typeof(ProblemDetails))]
        async public override Task<ActionResult<GetCategoryDto>> GetItem(int id)
        {
            return await base.GetItem(id);
        }

        [HttpPost]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Create a new category", Description = "Permission: Admin only")]
        [SwaggerResponse(201, "The category was created successfully", typeof(GetCategoryDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(409, "The category already exists", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetCategoryDto>> CreateItem([FromForm] CreateCategoryDto dto)
        {
            var validationResult = new CreateCategoryDtoValidator().Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var entity = await _categoryService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetItem), new { id = entity.Id }, entity);
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Update an existing category", Description = "Permission: Admin only")]
        [SwaggerResponse(200, "The category was updated successfully", typeof(GetCategoryDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The category was not found", typeof(ProblemDetails))]
        [SwaggerResponse(409, "The category already exists", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetCategoryDto>> PartialUpdateItem(int id, [FromForm] PartialUpdateCategoryDto dto)
        {
            var validationResult = new PartialUpdateCategoryDtoValidator().Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var updatedEntity = await _categoryService.UpdateAsync(dto, id);

            return Ok(updatedEntity);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Update an existing category", Description = "Permission: Admin only")]
        [SwaggerResponse(200, "The category was updated successfully", typeof(GetCategoryDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The was not found", typeof(ProblemDetails))]
        [SwaggerResponse(409, "The category already exists", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetCategoryDto>> UpdateItem(int id, [FromForm] UpdateCategoryDto dto)
        {
            var validationResult = new UpdateCategoryDtoValidator().Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var updatedEntity = await _categoryService.UpdateAsync(dto, id);

            return Ok(updatedEntity);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a category by id", Description = "Permission: Admin only")]
        [SwaggerResponse(204, "The category was deleted successfully")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The category was not found", typeof(ProblemDetails))]
        public override async Task<ActionResult<Category>> DeleteItem(int id)
        {
            return await base.DeleteItem(id);
        }
    }
}