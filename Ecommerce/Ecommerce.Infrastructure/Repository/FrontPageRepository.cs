using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Infrastructure.Database;

namespace Ecommerce.Infrastructure.Repository
{
    public class FrontPageRepository(EcommerceContext context) : BaseRepository<FrontPage, FrontPageFilterOptions>(context), IFrontPageRepo
    { }
}