using Ecommerce.Domain.Models;

namespace Ecommerce.Domain.Filters
{
    public class CategoryFilterOptions : PaginationOptionsBase<Category>
    {
        public int? ParentCategoryId { get; set; }
    }
}