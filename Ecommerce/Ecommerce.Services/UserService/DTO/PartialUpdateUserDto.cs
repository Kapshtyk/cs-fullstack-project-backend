using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.UserService.DTO
{
  public class PartialUpdateUserDto : IPartialUpdateDto<User>
  {
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Role? Role { get; set; }
    public string? Password { get; set; }
    public IFormFile? Avatar { get; set; }
    [SwaggerSchema(ReadOnly = true)]
    public string? AvatarPath { get; set; }

    public User GetUpdatedEntity(User entity)
    {
      entity.Email = Email ?? entity.Email;
      entity.FirstName = FirstName ?? entity.FirstName;
      entity.LastName = LastName ?? entity.LastName;
      entity.Role = Role ?? entity.Role;
      entity.Password = Password ?? entity.Password;
      entity.Avatar = AvatarPath ?? entity.Avatar;

      return entity;
    }
  }

  public class PartialUpdateUserDtoValidator : AbstractValidator<PartialUpdateUserDto>
  {
    public PartialUpdateUserDtoValidator()
    {
      RuleFor(x => x.Email).EmailAddress().When(x => x.Email != null);
      RuleFor(x => x.FirstName).MinimumLength(2).MaximumLength(100).When(x => x.FirstName != null);
      RuleFor(x => x.LastName).MinimumLength(2).MaximumLength(100).When(x => x.LastName != null);
      RuleFor(x => x.Password).MinimumLength(6).MaximumLength(20).When(x => x.Password != null);
    }
  }
}