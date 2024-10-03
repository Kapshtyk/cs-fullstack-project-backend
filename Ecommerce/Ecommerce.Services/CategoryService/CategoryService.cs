using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Services.CategoryService.DTO;
using Ecommerce.Services.CategoryService.Interfaces;
using Ecommerce.Services.Common;
using Ecommerce.Services.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Services.CategoryService
{
    public class CategoryService(
        ICategoryRepo categoryRepo,
        IDistributedCache cache,
        IFileService fileService
        ) :
        BaseService<Category, CategoryFilterOptions, GetCategoryDto, CreateCategoryDto, UpdateCategoryDto, PartialUpdateCategoryDto>(categoryRepo, cache),
        ICategoryService
    {
        private readonly IFileService _fileService = fileService;
        public override async Task<GetCategoryDto> CreateAsync(CreateCategoryDto createDto)
        {
            var relativeFilePath = await _fileService.SaveFileAsync(createDto.CategoryImage, "category");
            createDto.CategoryImagePath = relativeFilePath;
            try
            {
                return await base.CreateAsync(createDto);
            }
            catch (Exception)
            {
                await _fileService.DeleteFileAsync(relativeFilePath);
                throw;
            }
        }

        public override async Task<GetCategoryDto> UpdateAsync(PartialUpdateCategoryDto updateDto, int id)
        {
            if (updateDto.CategoryImage != null)
            {
                var relativeFilePath = await _fileService.SaveFileAsync(updateDto.CategoryImage, "category");
                updateDto.CategoryImagePath = relativeFilePath;
                try
                {
                    return await base.UpdateAsync(updateDto, id);
                }
                catch (Exception)
                {
                    await _fileService.DeleteFileAsync(relativeFilePath);
                    throw;
                }
            }
            else
            {
                return await base.UpdateAsync(updateDto, id);
            }
        }

        public override async Task<GetCategoryDto> UpdateAsync(UpdateCategoryDto updateDto, int id)
        {
            var relativeFilePath = await _fileService.SaveFileAsync(updateDto.CategoryImage, "category");
            updateDto.CategoryImagePath = relativeFilePath;

            try
            {
                return await base.UpdateAsync(updateDto, id);
            }
            catch (Exception)
            {
                await _fileService.DeleteFileAsync(relativeFilePath);
                throw;
            }
        }
    }
}