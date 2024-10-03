using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Models;
using Ecommerce.Services.ProductService.DTO;
using Ecommerce.Services.Common.Interfaces;

namespace Ecommerce.Services.ProductService.Interfaces
{
    public interface IProductService : IBaseService<Product, ProductFilterOptions, GetProductDto, PartialUpdateProductDto, CreateProductDto, UpdateProductDto>
    {
        public Task<IEnumerable<GetProductDto>> GetTopProducts(int numberOfProducts);
    }
}