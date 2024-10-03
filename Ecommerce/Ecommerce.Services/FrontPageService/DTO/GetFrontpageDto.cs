using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;

namespace Ecommerce.Services.FrontPageService.DTO
{
    public class GetFrontPageImageDto
    {
        public required string Url { get; set; }
    }
    public class GetFrontPageDto : IReadDto<FrontPage>
    {
        public int Id { get; set; }
        public required string HeroBanner { get; set; }
        public required string HeroBannerText { get; set; }
        public int SelectedProductId { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }


        public void FromEntity(FrontPage entity)
        {
            Id = entity.Id;
            HeroBanner = entity.HeroBanner;
            HeroBannerText = entity.HeroBannerText;
            SelectedProductId = entity.SelectedProductId;
            IsPublished = entity.IsPublished;
            CreatedAt = entity.CreatedAt;
        }
    }
}