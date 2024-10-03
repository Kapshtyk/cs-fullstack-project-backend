using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Models;
using Ecommerce.Services.ProductService.DTO;
using Ecommerce.Services.ProductService.Interfaces;
using Ecommerce.Services.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Services.Common;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    [SwaggerTag("Create, read, update and delete products")]
    public class ProductController(IProductService productService) : AppController<Product, ProductFilterOptions, GetProductDto, PartialUpdateProductDto, CreateProductDto, UpdateProductDto>(productService)
    {
        private readonly IProductService _productService = productService;

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get a list of products", Description = "Permission: All users")]
        [SwaggerResponse(200, "The products were fetched successfully", typeof(PaginatedResult<Product, GetProductDto>))]
        [SwaggerResponse(401, "Unauthorized")]
        async public override Task<ActionResult<PaginatedResult<Product, GetProductDto>>> GetItems([FromQuery] ProductFilterOptions filteringOptions)
        {
            return await base.GetItems(filteringOptions);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get a product by id", Description = "Permission: All users")]
        [SwaggerResponse(200, "The product was fetched successfully", typeof(GetProductDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "The product was not found", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetProductDto>> GetItem(int id)
        {
            return await base.GetItem(id);
        }

        [HttpGet]
        [Route("top-products")]
        [AllowAnonymous]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get top products", Description = "Permission: All users")]
        [SwaggerResponse(200, "The product was fetched successfully", typeof(GetProductDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "The product was not found", typeof(ProblemDetails))]
        public async Task<ActionResult<GetProductDto>> GetTopProducts([FromQuery] int numberOfProducts = 5)
        {
            var products = await _productService.GetTopProducts(numberOfProducts);

            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Create a new product", Description = "Permission: Admin only")]
        [SwaggerResponse(201, "The product was created successfully", typeof(GetProductDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(409, "The product already exists", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetProductDto>> CreateItem([FromForm] CreateProductDto dto)
        {
            var validationResult = new CreateProductDtoValidator().Validate(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var entity = await _productService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetItem), new { id = entity.Id }, entity);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Update an existing product", Description = "Permission: Admin only")]
        [SwaggerResponse(200, "The product was updated successfully", typeof(GetProductDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The product was not found", typeof(ProblemDetails))]
        [SwaggerResponse(409, "The product already exists", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetProductDto>> PartialUpdateItem(int id, [FromForm] PartialUpdateProductDto dto)
        {
            var validationResult = new PartialUpdateProductDtoValidator().Validate(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatedEntity = await _productService.UpdateAsync(dto, id);

            return Ok(updatedEntity);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Update an existing product", Description = "Permission: Admin only")]
        [SwaggerResponse(200, "The product was updated successfully", typeof(GetProductDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The product was not found", typeof(ProblemDetails))]
        [SwaggerResponse(409, "The product already exists", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetProductDto>> UpdateItem(int id, [FromForm] UpdateProductDto dto)
        {
            var validationResult = new UpdateProductDtoValidator().Validate(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var entity = await _productService.UpdateAsync(dto, id);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a product by id", Description = "Permission: Admin only")]
        [SwaggerResponse(204, "The product was deleted successfully")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The product was not found", typeof(ProblemDetails))]
        public override async Task<ActionResult<Product>> DeleteItem(int id)
        {
            return await base.DeleteItem(id);
        }
    }
}
