using System.Linq.Expressions;
using Ecommerce.Domain.Models;

namespace Ecommerce.Domain.Filters
{
    public class ReviewFilterOptions : PaginationOptionsBase<Review>
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }

        public override Expression<Func<Review, bool>> GetFilterExpression()
        {
            return e => (ProductId == 0 || e.ProductId == ProductId) &&
                        (UserId == 0 || e.UserId == UserId);
        }
    }
}