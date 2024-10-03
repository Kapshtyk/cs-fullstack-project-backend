using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using Ecommerce.Services.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.CategoryService.DTO
{
    public class CreateCategoryDto : ICreateDto<Category>
    {
        public required string Name { get; set; }
        public required IFormFile CategoryImage { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public required string CategoryImagePath { get; set; }
        public int? ParentCategoryId { get; set; }

        public Category GetEntity()
        {
            return new Category
            {
                Name = Name,
                CategoryImage = CategoryImagePath,
                ParentCategoryId = ParentCategoryId,
            };
        }

        public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
        {
            public CreateCategoryDtoValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(50);
                RuleFor(x => x.CategoryImage).NotNull();
                RuleFor(x => x.ParentCategoryId).GreaterThan(0);
            }
        }
    }
}