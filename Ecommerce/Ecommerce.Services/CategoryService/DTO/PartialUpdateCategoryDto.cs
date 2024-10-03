using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using Ecommerce.Services.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.CategoryService.DTO
{
    public class PartialUpdateCategoryDto() : IPartialUpdateDto<Category>
    {
        public string? Name { get; set; }
        public IFormFile? CategoryImage { get; set; }
        public string? CategoryImagePath { get; set; }
        public int? ParentCategoryId { get; set; }

        public Category GetUpdatedEntity(Category entity)
        {
            entity.Name = Name ?? entity.Name;
            entity.CategoryImage = CategoryImagePath ?? entity.CategoryImage;
            entity.ParentCategoryId = ParentCategoryId ?? entity.ParentCategoryId;

            return entity;
        }
    }

    public class PartialUpdateCategoryDtoValidator : AbstractValidator<PartialUpdateCategoryDto>
    {
        public PartialUpdateCategoryDtoValidator()
        {
            RuleFor(x => x.Name).MinimumLength(3).MaximumLength(50).When(x => x.Name != null);
        }
    }

}