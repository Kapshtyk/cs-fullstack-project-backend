using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using Ecommerce.Services.ProductService.DTO;
using Ecommerce.Services.UserService.DTO;

namespace Ecommerce.Services.ReviewService.DTO
{

    public class GetReviewDto : IReadDto<Review>
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required int Rating { get; set; }
        public required GetUserDto User { get; set; }
        public required GetProductDto Product { get; set; }

        public void FromEntity(Review entity)
        {
            var user = new GetUserDto();
            user.FromEntity(entity.User);
            var product = new GetProductDto();
            product.FromEntity(entity.Product);

            Id = entity.Id;
            Title = entity.Title;
            Description = entity.Description;
            Rating = entity.Rating;
            User = user;
            Product = product;
        }
    }
}