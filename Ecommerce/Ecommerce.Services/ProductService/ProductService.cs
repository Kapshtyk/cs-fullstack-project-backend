using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Services.ProductService.DTO;
using Ecommerce.Services.ProductService.Interfaces;
using Ecommerce.Services.Common;
using Microsoft.Extensions.Caching.Distributed;
using Ecommerce.Services.Extensions;
using Ecommerce.Services.Common.Interfaces;

namespace Ecommerce.Services.ProductService
{
    public class ProductService(
        IProductRepo productRepo,
        IDistributedCache cache,
        IFileService fileService
        ) :
        BaseService<Product, ProductFilterOptions, GetProductDto, CreateProductDto, UpdateProductDto, PartialUpdateProductDto>(productRepo, cache),
        IProductService
    {
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IFileService _fileService = fileService;

        public override async Task<GetProductDto> CreateAsync(CreateProductDto createDto)
        {
            foreach (var image in createDto.ProductImage)
            {
                var imagePath = await _fileService.SaveFileAsync(image, "products");
                createDto.ProductImagePath.Add(imagePath);
            }

            return await base.CreateAsync(createDto);
        }

        public override async Task<GetProductDto> UpdateAsync(UpdateProductDto updateDto, int id)
        {
            foreach (var image in updateDto.ProductImage)
            {
                var imagePath = await _fileService.SaveFileAsync(image, "products");
                updateDto.ProductImagePath.Add(imagePath);
            }

            return await base.UpdateAsync(updateDto, id);
        }

        public override async Task<GetProductDto> UpdateAsync(PartialUpdateProductDto updateDto, int id)
        {
            if (updateDto.ProductImage != null)
            {
                updateDto.ProductImagePath ??= [];
                foreach (var image in updateDto.ProductImage)
                {
                    var imagePath = await _fileService.SaveFileAsync(image, "products");
                    updateDto.ProductImagePath.Add(imagePath);
                }
            }

            return await base.UpdateAsync(updateDto, id);
        }

        public async Task<IEnumerable<GetProductDto>> GetTopProducts(int numberOfProducts)
        {
            var products = await _productRepo.GetTopProducts(numberOfProducts);
            var productDtos = new List<GetProductDto>();

            foreach (var product in products)
            {
                var productDto = new GetProductDto();
                productDto.FromEntity(product);
                productDtos.Add(productDto);
            }

            return productDtos;
        }
    }
}