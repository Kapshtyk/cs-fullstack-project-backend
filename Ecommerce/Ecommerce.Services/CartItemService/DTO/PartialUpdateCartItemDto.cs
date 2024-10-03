using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;

namespace Ecommerce.Services.CartItemService.DTO
{
    public class PartialUpdateCartItemDto : IPartialUpdateDto<CartItem>
    {
        public required int Quantity { get; set; }

        public CartItem GetUpdatedEntity(CartItem entity)
        {
            entity.Quantity = Quantity;
            return entity;
        }
    }

    public class PartialUpdateCartItemDtoValidator : AbstractValidator<PartialUpdateCartItemDto>
    {
        public PartialUpdateCartItemDtoValidator()
        {
            RuleFor(x => x.Quantity).NotNull().GreaterThan(0);
        }
    }
}