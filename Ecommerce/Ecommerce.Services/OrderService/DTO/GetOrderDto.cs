using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using Ecommerce.Services.ProductService.DTO;

namespace Ecommerce.Services.OrderService.DTO
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public required GetProductDto Product { get; set; }
    }

    public class GetOrderDto : IReadDto<Order>
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required DateTime OrderDate { get; set; }
        public required ICollection<OrderItemDto> OrderItems { get; set; }

        public void FromEntity(Order entity)
        {
            Id = entity.Id;
            UserId = entity.UserId;
            OrderDate = entity.OrderDate;
            OrderItems = entity.OrderItems?.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                Quantity = oi.Quantity,
                Price = oi.Price,
                Product = new GetProductDto
                {
                    Id = oi.Product.Id,
                    Title = oi.Product.Title,
                    Description = oi.Product.Description,
                    Price = oi.Product.Price,
                    Stock = oi.Product.Stock,
                    CategoryId = oi.Product.CategoryId,
                    ProductImage = oi.Product.ProductImages?.Select(pi => new GetProducImageDto
                    {
                        Url = pi.Url
                    }).ToList() ?? [],
                },
            }).ToList() ?? [];
        }
    }
}