using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.FrontPageService.DTO
{
    public class CreateFrontPageDto : ICreateDto<FrontPage>
    {
        public required string HeroBannerText { get; set; }
        public int SelectedProductId { get; set; }
        public required IFormFile HeroBannerImage { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public required string HeroBannerImagePath { get; set; }
        public bool IsPublished { get; set; } = false;

        public FrontPage GetEntity()
        {
            return new FrontPage
            {
                HeroBannerText = HeroBannerText,
                HeroBanner = HeroBannerImagePath,
                IsPublished = IsPublished,
                SelectedProductId = SelectedProductId
            };
        }
    }

    public class CreateFrontPageDtoValidator : AbstractValidator<CreateFrontPageDto>
    {
        public CreateFrontPageDtoValidator()
        {
            RuleFor(x => x.HeroBannerText).NotEmpty().Length(3, 256);
            RuleFor(x => x.SelectedProductId).NotEmpty().GreaterThan(0);
            RuleFor(x => x.HeroBannerImage).NotEmpty();
        }
    }
}