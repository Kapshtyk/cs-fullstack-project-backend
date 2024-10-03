using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;

namespace Ecommerce.Services.ReviewService.DTO
{
    public class UpdateReviewDto : IUpdateDto<Review>
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required int Rating { get; set; }
        public required int UserId { get; set; }
        public required int ProductId { get; set; }

        public Review GetUpdatedEntity(Review entity)
        {
            entity.Title = Title;
            entity.Description = Description;
            entity.Rating = Rating;
            entity.UserId = UserId;
            entity.ProductId = ProductId;

            return entity;
        }
    }

    public class UpdateReviewDtoValidator : AbstractValidator<UpdateReviewDto>
    {
        public UpdateReviewDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MinimumLength(3).MaximumLength(500);
            RuleFor(x => x.Rating).NotEmpty().GreaterThanOrEqualTo(0).LessThanOrEqualTo(5);
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}