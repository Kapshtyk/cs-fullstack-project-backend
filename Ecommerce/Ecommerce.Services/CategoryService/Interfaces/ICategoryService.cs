using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Models;
using Ecommerce.Services.CategoryService.DTO;
using Ecommerce.Services.Common.Interfaces;

namespace Ecommerce.Services.CategoryService.Interfaces
{
    public interface ICategoryService : IBaseService<Category, CategoryFilterOptions, GetCategoryDto, PartialUpdateCategoryDto, CreateCategoryDto, UpdateCategoryDto>
    { }
}