using Ecommerce.Domain.Common;
using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ecommerce.Infrastructure.Repository
{
    public class BaseRepository<T, TFilter>(EcommerceContext context) : IBaseRepo<T, TFilter>
        where T : BaseEntity
        where TFilter : PaginationOptionsBase<T>
    {
        protected readonly EcommerceContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public virtual async Task<int> CountAsync(TFilter filteringOptions)
        {
            return await _dbSet.CountAsync(filteringOptions.GetFilterExpression());
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            _dbSet.Add(entity);

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

            return entity;
        }

        public virtual async Task<bool> DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id) ?? throw new EntityNotFoundException<T>();
            try
            {
                var state = _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(TFilter filteringOptions)
        {
            var result = _dbSet.Where(filteringOptions.GetFilterExpression());

            return await result
                .OrderBy(e => e.Id)
                .Skip(filteringOptions.PerPage * (filteringOptions.Page - 1))
                .Take(filteringOptions.PerPage)
                .ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id) ?? throw new EntityNotFoundException<T>();
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);

            try
            {
                await _context.SaveChangesAsync();
                return entity;
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