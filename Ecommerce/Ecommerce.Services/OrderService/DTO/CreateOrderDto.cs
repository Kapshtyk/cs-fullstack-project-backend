using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;

namespace Ecommerce.Services.OrderService.DTO
{
    public class CreateOrderDto : ICreateDto<Order>
    {
        public required int UserId { get; set; }

        public Order GetEntity()
        {
            return new Order
            {
                UserId = UserId,
                User = null!,
                OrderItems = null!

            };
        }
    }

    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}