using Ecommerce.Domain.Models;

namespace Ecommerce.Domain.Filters
{
    public class CartItemFilterOptions : PaginationOptionsBase<CartItem>
    {
        public required int UserId { get; set; }
    }
}