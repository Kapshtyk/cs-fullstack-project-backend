using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ecommerce.Infrastructure.Repository
{
    public class CartItemRepository(EcommerceContext context) : BaseRepository<CartItem, CartItemFilterOptions>(context), ICartItemRepo
    {
        public override async Task<CartItem> CreateAsync(CartItem entity)
        {
            try
            {
                return await _context
                    .CreateCartItem(entity.UserId, entity.ProductId, entity.Quantity)
                    .Include(c => c.Product)
                    .ThenInclude(p => p.ProductImages)
                    .FirstAsync();
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "P0001")
                {
                    switch (ex.MessageText)
                    {
                        case "Invalid user id":
                            throw new EntityNotFoundException<User>();
                        case "Invalid product id":
                            throw new EntityNotFoundException<Product>();
                        default:
                            throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        public override async Task<IEnumerable<CartItem>> GetAllAsync(CartItemFilterOptions filteringOptions)
        {
            return await _context.CartItems
                .Where(filteringOptions.GetFilterExpression())
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .OrderBy(c => c.Id)
                .Skip(filteringOptions.PerPage * (filteringOptions.Page - 1))
                .Take(filteringOptions.PerPage)
                .ToListAsync();
        }

        public override async Task<CartItem> GetByIdAsync(int id)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == id) ?? throw new EntityNotFoundException<CartItem>();
        }

        public override async Task<CartItem> UpdateAsync(CartItem entity)
        {
            _dbSet.Update(entity);

            try
            {
                await _context.SaveChangesAsync();
                var updatedCartItem = await _context.CartItems
                    .Include(c => c.Product)
                    .ThenInclude(p => p.ProductImages)
                    .FirstOrDefaultAsync(c => c.Id == entity.Id) ?? throw new EntityNotFoundException<CartItem>();
                return updatedCartItem;
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
        }
    }
}