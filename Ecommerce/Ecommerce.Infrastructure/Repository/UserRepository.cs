using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repository
{
	public class UserRepository(EcommerceContext context) : BaseRepository<User, UserFilterOptions>(context), IUserRepo
	{

		public override async Task<User> CreateAsync(User entity)
		{
			if (!await _context.Users.AnyAsync())
			{
				entity.Role = Role.Admin;
			}
			return await base.CreateAsync(entity);
		}

		public Task<User?> GetUserByEmailAsync(string email)
		{
			return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}
	}
}