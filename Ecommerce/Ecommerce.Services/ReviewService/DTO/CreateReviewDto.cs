using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;

namespace Ecommerce.Services.ReviewService.DTO
{
    public class CreateReviewDto : ICreateDto<Review>
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required int Rating { get; set; }
        public required int UserId { get; set; }
        public required int ProductId { get; set; }

        public Review GetEntity()
        {
            return new Review
            {
                Title = Title,
                Description = Description,
                Rating = Rating,
                UserId = UserId,
                ProductId = ProductId
            };
        }
    }

    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MinimumLength(3).MaximumLength(500);
            RuleFor(x => x.Rating).NotEmpty().GreaterThanOrEqualTo(1).LessThanOrEqualTo(5);
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}