using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;

namespace Ecommerce.Services.ProductService.DTO
{
    public class GetProducImageDto
    {
        public required string Url { get; set; }
    }
    public class GetProductDto : IReadDto<Product>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public List<GetProducImageDto> ProductImage { get; set; } = [];

        public void FromEntity(Product entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Description = entity.Description;
            Price = entity.Price;
            Stock = entity.Stock;
            CategoryId = entity.CategoryId;
            ProductImage = entity.ProductImages?.Select(x => new GetProducImageDto
            { Url = x.Url }).ToList() ?? [];
        }
    }
}