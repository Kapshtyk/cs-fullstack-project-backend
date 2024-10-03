using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;

namespace Ecommerce.Services.CartItemService.DTO
{
    public class CreateCartItemDto : ICreateDto<CartItem>
    {
        public required int Quantity { get; set; }
        public required int UserId { get; set; }
        public required int ProductId { get; set; }

        public CartItem GetEntity()
        {
            return new CartItem
            {
                Id = 0,
                Quantity = Quantity,
                UserId = UserId,
                ProductId = ProductId,
                User = null!,
                Product = null!
            };
        }

        public class CreateCartItemDtoValidator : AbstractValidator<CreateCartItemDto>
        {
            public CreateCartItemDtoValidator()
            {
                RuleFor(x => x.Quantity).GreaterThan(0);
            }
        }
    }
}