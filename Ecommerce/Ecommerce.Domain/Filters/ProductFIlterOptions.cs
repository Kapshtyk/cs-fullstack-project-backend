using Ecommerce.Domain.Models;

namespace Ecommerce.Domain.Filters
{
    public class ProductFilterOptions : PaginationOptionsBase<Product>
    {
        public int? CategoryId { get; set; }
    }
}