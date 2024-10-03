using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Models;

namespace Ecommerce.Domain.Interfaces
{
    public interface IFrontPageRepo :
        IBaseRepo<FrontPage, FrontPageFilterOptions>
    { }
};
