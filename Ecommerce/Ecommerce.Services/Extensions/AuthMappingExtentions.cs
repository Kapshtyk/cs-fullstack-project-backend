using Ecommerce.Domain.Models;
using Ecommerce.Services.AuthService;

namespace Ecommerce.Services.Extensions
{
  public static class AuthMappingExtentions
  {
    public static TokenOptions ToTokenOptions(this User entity)
    {
      return new TokenOptions
      {
        Id = entity.Id,
        Email = entity.Email,
        Role = entity.Role
      };
    }
  }
}