using System.Linq.Expressions;
using Ecommerce.Domain.Models;

namespace Ecommerce.Domain.Filters
{
    public class FrontPageFilterOptions : PaginationOptionsBase<FrontPage>
    {
        public bool? IsPublished { get; set; }

        public override Expression<Func<FrontPage, bool>> GetFilterExpression()
        {
            return e => IsPublished == null || e.IsPublished == IsPublished;
        }
    }
}