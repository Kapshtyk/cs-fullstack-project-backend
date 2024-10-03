using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ecommerce.Infrastructure.Repository
{
    public class OrderRepository(EcommerceContext context) : BaseRepository<Order, OrderFilterOptions>(context), IOrderRepo
    {
        public override async Task<Order> CreateAsync(Order entity)
        {
            try
            {
                var newOrder = await _context.CreateOrderFromCart(entity.UserId).FirstOrDefaultAsync() ?? throw new OrderNotFoundException();
                return await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductImages)
                    .FirstOrDefaultAsync(o => o.Id == newOrder.Id) ?? throw new OrderNotFoundException();
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "P0001")
                {
                    throw new EntityNotFoundException<Order>();
                }
                if (ex.SqlState == "P0002")
                {
                    throw new InsufficientStockException();
                }
                throw;
            }
        }

        public override async Task<bool> DeleteByIdAsync(int id)
        {
            var _ = await GetByIdAsync(id) ?? throw new EntityNotFoundException<Order>();

            string query = "SELECT * FROM delete_order({0})";
            object[] parameters = [id];

            try
            {
                var count = await _context.Database
                    .SqlQueryRaw<bool>(query, parameters)
                    .ToListAsync();

                return count.FirstOrDefault();
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "P0001")
                {
                    throw new EntityNotFoundException<Order>();
                }
                throw;
            }
        }

        public override async Task<IEnumerable<Order>> GetAllAsync(OrderFilterOptions filteringOptions)
        {
            return await _context.Orders
                .Where(filteringOptions.GetFilterExpression())
                .Include(c => c.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.ProductImages)
                .OrderBy(c => c.Id)
                .Skip(filteringOptions.PerPage * (filteringOptions.Page - 1))
                .Take(filteringOptions.PerPage)
                .ToListAsync();
        }

        public override async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(c => c.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == id) ?? throw new EntityNotFoundException<Order>();
        }

        public override async Task<Order> UpdateAsync(Order entity)
        {
            _dbSet.Update(entity);


            await _context.SaveChangesAsync();
            var updatedOrder = await _context.Orders
                .Include(c => c.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == entity.Id) ?? throw new EntityNotFoundException<Order>();
            return updatedOrder;
        }
    }
}