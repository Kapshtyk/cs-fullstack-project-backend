using System.Linq.Expressions;
using Ecommerce.Domain.Models;

namespace Ecommerce.Domain.Filters
{
    public class OrderFilterOptions : PaginationOptionsBase<Order>
    {
        public int? UserId { get; set; }

        public override Expression<Func<Order, bool>> GetFilterExpression()
        {
            return e => UserId == null || e.UserId == UserId;
        }
    }
}