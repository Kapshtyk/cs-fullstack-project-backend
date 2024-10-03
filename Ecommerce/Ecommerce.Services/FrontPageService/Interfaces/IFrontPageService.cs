using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.Interfaces;
using Ecommerce.Services.FrontPageService.DTO;

namespace Ecommerce.Services.FrontPageService.Interfaces
{
    public interface IFrontPageService : IBaseService<FrontPage, FrontPageFilterOptions, GetFrontPageDto, PartialUpdateFrontPageDto, CreateFrontPageDto, UpdateFrontPageDto>
    { }
}