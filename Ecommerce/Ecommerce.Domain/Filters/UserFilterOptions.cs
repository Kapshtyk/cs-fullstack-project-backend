using System.Linq.Expressions;
using Ecommerce.Domain.Models;

namespace Ecommerce.Domain.Filters
{
    public class UserFilterOptions : PaginationOptionsBase<User>
    {
        public Role? Role { get; set; }

        public override Expression<Func<User, bool>> GetFilterExpression()
        {
            return e => Role == null || e.Role == Role;
        }
    }
}