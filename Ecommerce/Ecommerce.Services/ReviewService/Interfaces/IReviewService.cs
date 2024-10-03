using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Models;
using Ecommerce.Services.ReviewService.DTO;
using Ecommerce.Services.Common.Interfaces;

namespace Ecommerce.Services.ReviewService.Interfaces
{
    public interface IReviewService : IBaseService<Review, ReviewFilterOptions, GetReviewDto, PartialUpdateReviewDto, CreateReviewDto, UpdateReviewDto>
    { }
}