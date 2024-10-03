using System.Linq.Expressions;
using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Filters
{
    public abstract class PaginationOptionsBase<T>
        where T : BaseEntity
    {
        public int Page { get; set; } = 1;
        public int PerPage { get; set; } = 10;

        public virtual Expression<Func<T, bool>> GetFilterExpression()
        {
            return e => true;
        }
    }
}