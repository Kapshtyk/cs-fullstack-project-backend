using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repository
{
    public class CategoryRepository(EcommerceContext context) : BaseRepository<Category, CategoryFilterOptions>(context), ICategoryRepo
    {
        public override async Task<int> CountAsync(CategoryFilterOptions filteringOptions)
        {
            string query;
            object[] parameters;

            if (filteringOptions.ParentCategoryId.HasValue)
            {
                query = "SELECT * FROM count_categories({0})";
                parameters = [filteringOptions.ParentCategoryId.Value];
            }
            else
            {
                query = "SELECT * FROM count_categories()";
                parameters = [];
            }

            var count = await _context.Database
                .SqlQueryRaw<int>(query, parameters)
                .ToListAsync();

            return count.FirstOrDefault();
        }
        public override async Task<IEnumerable<Category>> GetAllAsync(CategoryFilterOptions filteringOptions)
        {
            return await _context.GetCategories(filteringOptions.Page, filteringOptions.PerPage, filteringOptions.ParentCategoryId).OrderBy(c => c.Id).ToListAsync();
        }
    }
}