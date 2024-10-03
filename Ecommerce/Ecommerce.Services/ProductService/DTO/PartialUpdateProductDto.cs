using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.ProductService.DTO
{
    public class PartialUpdateProductDto() : IPartialUpdateDto<Product>
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public int? CategoryId { get; set; }
        public List<IFormFile>? ProductImage { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public List<string>? ProductImagePath { get; set; }

        public Product GetUpdatedEntity(Product entity)
        {
            entity.Title = Title ?? entity.Title;
            entity.Description = Description ?? entity.Description;
            entity.Price = Price ?? entity.Price;
            entity.Stock = Stock ?? entity.Stock;
            entity.CategoryId = CategoryId ?? entity.CategoryId;
            entity.ProductImages = ProductImagePath?.Select(x => new ProductImage { Url = x, Alt = string.Empty }).ToList() ?? entity.ProductImages;

            return entity;
        }
    }

    public class PartialUpdateProductDtoValidator : AbstractValidator<PartialUpdateProductDto>
    {
        public PartialUpdateProductDtoValidator()
        {
            RuleFor(x => x.Title).MinimumLength(3).MaximumLength(50).When(x => x.Title != null);
            RuleFor(x => x.Description).MinimumLength(3).MaximumLength(500).When(x => x.Description != null);
            RuleFor(x => x.Price).GreaterThan(0).When(x => x.Price != null);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).When(x => x.Stock != null);
            RuleFor(x => x.CategoryId).GreaterThanOrEqualTo(1).When(x => x.CategoryId != null);
            RuleFor(x => x.ProductImage).Must(x => x.Count > 0).WithMessage("At least one image is required").When(x => x.ProductImage != null);
        }
    }
}