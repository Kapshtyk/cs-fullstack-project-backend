using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;

namespace Ecommerce.Services.OrderService.DTO
{
    public class PartialUpdateOrderDto : IPartialUpdateDto<Order>
    {
        public required int UserId { get; set; }

        public Order GetUpdatedEntity(Order entity)
        {
            entity.UserId = UserId;
            return entity;
        }
    }

    public class PartialUpdateOrderDtoValidator : AbstractValidator<PartialUpdateOrderDto>
    {
        public PartialUpdateOrderDtoValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}