using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using Ecommerce.Services.ProductService.DTO;

namespace Ecommerce.Services.CartItemService.DTO
{
    public class GetCartItemDto : IReadDto<CartItem>
    {
        public int Id { get; set; }
        public required int Quantity { get; set; }
        public required int UserId { get; set; }
        public required GetProductDto Product { get; set; }

        public void FromEntity(CartItem entity)
        {
            var productDto = new GetProductDto();
            productDto.FromEntity(entity.Product);

            Id = entity.Id;
            Quantity = entity.Quantity;
            UserId = entity.UserId;
            Product = productDto;
        }
    }
}