using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ecommerce.Infrastructure.Repository
{
    public class ProductRepository(EcommerceContext context) : BaseRepository<Product, ProductFilterOptions>(context), IProductRepo
    {
        public override async Task<int> CountAsync(ProductFilterOptions filteringOptions)
        {
            string query;
            object[] parameters;

            if (filteringOptions.CategoryId.HasValue)
            {
                query = "SELECT * FROM count_products({0})";
                parameters = [filteringOptions.CategoryId.Value];
            }
            else
            {
                query = "SELECT * FROM count_products()";
                parameters = [];
            }

            var count = await _context.Database
                .SqlQueryRaw<int>(query, parameters)
                .ToListAsync();

            return count.FirstOrDefault();
        }
        public override async Task<Product> CreateAsync(Product entity)
        {
            var result = _context.Products.Add(entity);

            foreach (var productImage in entity.ProductImages)
            {
                productImage.ProductId = result.Entity.Id;
                _context.ProductImages.Add(productImage);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var exc = ex.InnerException;

                if (exc is PostgresException postgresException)
                {
                    throw new ContstraintViolationException(postgresException.MessageText);
                }
                else
                {
                    throw;
                }
            }
            return result.Entity;
        }

        public override async Task<IEnumerable<Product>> GetAllAsync(ProductFilterOptions filteringOptions)
        {
            return await _context.GetProducts(filteringOptions.Page, filteringOptions.PerPage, filteringOptions.CategoryId)
            .Include(p => p.ProductImages)
            .OrderBy(c => c.Id)
            .ToListAsync();
        }

        public override async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException<Product>();
        }

        public async Task<IEnumerable<Product>> GetTopProducts(int numberOfProducts)
        {
            return await _context.GetTopProducts(numberOfProducts)
                .Include(p => p.ProductImages)
                .ToListAsync();
        }

        public override async Task<Product> UpdateAsync(Product entity)
        {
            var existingProduct = await _dbSet
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == entity.Id) ?? throw new EntityNotFoundException<Product>();

            _dbSet.Entry(existingProduct).CurrentValues.SetValues(entity);

            var ProductImages = new List<ProductImage>();

            foreach (var productImage in entity.ProductImages)
            {
                productImage.ProductId = existingProduct.Id;
                ProductImages.Add(productImage);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var exc = ex.InnerException;

                if (exc is PostgresException postgresException)
                {
                    throw new ContstraintViolationException(postgresException.MessageText);
                }
                else
                {
                    throw;
                }
            }

            return await _dbSet
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == entity.Id)
                ?? throw new EntityNotFoundException<Product>();
        }
    }
}