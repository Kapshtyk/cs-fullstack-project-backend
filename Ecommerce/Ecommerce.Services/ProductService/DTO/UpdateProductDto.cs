using System.Data;
using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.ProductService.DTO
{
    public class UpdateProductDto : IUpdateDto<Product>
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required int Stock { get; set; }
        public required int CategoryId { get; set; }
        public List<IFormFile> ProductImage { get; set; } = [];
        [SwaggerSchema(ReadOnly = true)]
        public List<string> ProductImagePath { get; set; } = [];

        public Product GetUpdatedEntity(Product entity)
        {
            entity.Title = Title;
            entity.Description = Description;
            entity.Price = Price;
            entity.Stock = Stock;
            entity.CategoryId = CategoryId;
            entity.ProductImages = ProductImagePath?.Select(x => new ProductImage { Url = x, Alt = string.Empty }).ToList() ?? [];

            return entity;
        }
    }

    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.ProductImage).Must(x => x.Count > 0).WithMessage("At least one image is required");
            RuleFor(x => x.Title).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MinimumLength(3).MaximumLength(500);
            RuleFor(x => x.Price).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Stock).NotEmpty().GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}