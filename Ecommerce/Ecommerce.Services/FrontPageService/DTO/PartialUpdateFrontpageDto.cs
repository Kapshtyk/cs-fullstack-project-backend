using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.FrontPageService.DTO
{
    public class PartialUpdateFrontPageDto : IPartialUpdateDto<FrontPage>
    {
        public required string HeroBannerText { get; set; }
        public int SelectedProductId { get; set; }
        public required IFormFile HeroBannerImage { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public required string HeroBannerImagePath { get; set; }
        public bool IsPublished { get; set; }

        public FrontPage GetUpdatedEntity(FrontPage entity)
        {
            entity.HeroBannerText = HeroBannerText ?? entity.HeroBannerText;
            entity.SelectedProductId = SelectedProductId == 0 ? entity.SelectedProductId : SelectedProductId;
            entity.HeroBanner = HeroBannerImagePath ?? entity.HeroBanner;
            entity.IsPublished = IsPublished == false ? entity.IsPublished : IsPublished;

            return entity;
        }
    }

    public class PartialUpdateFrontPageDtoValidator : AbstractValidator<PartialUpdateFrontPageDto>
    {
        public PartialUpdateFrontPageDtoValidator()
        {
            RuleFor(x => x.HeroBannerText).Length(3, 256).When(x => x.HeroBannerText != null);
            RuleFor(x => x.HeroBannerImagePath).Length(3, 256).When(x => x.HeroBannerImagePath != null);
            RuleFor(x => x.SelectedProductId).GreaterThan(0).When(x => x.SelectedProductId != 0);
        }
    }
}