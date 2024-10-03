using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using Ecommerce.Services.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.CategoryService.DTO
{
    public class UpdateCategoryDto : IUpdateDto<Category>
    {
        public required string Name { get; set; }
        public required IFormFile CategoryImage { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public required string CategoryImagePath { get; set; }
        public int? ParentCategoryId { get; set; }

        public Category GetUpdatedEntity(Category entity)
        {
            entity.Name = Name;
            entity.CategoryImage = CategoryImagePath;
            entity.ParentCategoryId = ParentCategoryId;

            return entity;
        }

        public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
        {
            public UpdateCategoryDtoValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(50);
                RuleFor(x => x.CategoryImage).NotNull();
                RuleFor(x => x.ParentCategoryId).GreaterThan(0);
            }
        }
    }
}