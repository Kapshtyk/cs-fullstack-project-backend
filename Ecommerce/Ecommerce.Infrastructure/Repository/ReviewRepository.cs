using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repository
{
    public class ReviewRepository(EcommerceContext context) : BaseRepository<Review, ReviewFilterOptions>(context), IReviewRepo
    {
        public override async Task<Review> CreateAsync(Review entity)
        {
            var result = await _context.Reviews.AddAsync(entity);
            await _context.SaveChangesAsync();
            return await _context.Reviews
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == result.Entity.Id) ?? throw new EntityNotFoundException<Review>();
        }

        public override async Task<IEnumerable<Review>> GetAllAsync(ReviewFilterOptions filteringOptions)
        {
            return await _context.Reviews
                .Where(filteringOptions.GetFilterExpression())
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(c => c.User)
                .OrderBy(c => c.Id)
                .Skip(filteringOptions.PerPage * (filteringOptions.Page - 1))
                .Take(filteringOptions.PerPage)
                .ToListAsync();
        }

        public override async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id) ?? throw new EntityNotFoundException<Review>();
        }

        public override async Task<Review> UpdateAsync(Review entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            var updatedReview = await _context.Reviews
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == entity.Id) ?? throw new EntityNotFoundException<Review>();
            return updatedReview;
        }
    }
}