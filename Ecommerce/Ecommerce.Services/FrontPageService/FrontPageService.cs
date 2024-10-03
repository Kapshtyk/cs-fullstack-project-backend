using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Services.Common;
using Ecommerce.Services.Common.Interfaces;
using Ecommerce.Services.FrontPageService.DTO;
using Ecommerce.Services.FrontPageService.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Services.FrontPageService
{
    public class FrontPageService(
        IFrontPageRepo frontPageRepo,
        IDistributedCache cache,
        IProductRepo productRepo,
        IFileService fileService
        ) :
        BaseService<FrontPage, FrontPageFilterOptions, GetFrontPageDto, CreateFrontPageDto, UpdateFrontPageDto, PartialUpdateFrontPageDto>(frontPageRepo, cache),
        IFrontPageService
    {
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IFileService _fileService = fileService;
        public override async Task<GetFrontPageDto> CreateAsync(CreateFrontPageDto createDto)
        {
            var product = await _productRepo.GetByIdAsync(createDto.SelectedProductId) ?? throw new EntityNotFoundException<Product>();
            var imagePath = await _fileService.SaveFileAsync(createDto.HeroBannerImage, "frontpage");
            createDto.HeroBannerImagePath = imagePath;

            return await base.CreateAsync(createDto);
        }

        public override async Task<GetFrontPageDto> UpdateAsync(PartialUpdateFrontPageDto updateDto, int id)
        {
            var product = await _productRepo.GetByIdAsync(updateDto.SelectedProductId) ?? throw new EntityNotFoundException<Product>();
            if (updateDto.HeroBannerImage != null)
            {
                var imagePath = await _fileService.SaveFileAsync(updateDto.HeroBannerImage, "frontpage");
                updateDto.HeroBannerImagePath = imagePath;
            }

            return await base.UpdateAsync(updateDto, id);
        }
    }
}