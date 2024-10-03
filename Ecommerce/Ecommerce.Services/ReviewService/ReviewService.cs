using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Services.ReviewService.DTO;
using Ecommerce.Services.ReviewService.Interfaces;
using Ecommerce.Services.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Services.ReviewService
{
    public class ReviewService(
        IReviewRepo reviewRepo,
        IDistributedCache cache
        ) : BaseService<Review, ReviewFilterOptions, GetReviewDto, CreateReviewDto, UpdateReviewDto, PartialUpdateReviewDto>(reviewRepo, cache),
        IReviewService
    { }
}