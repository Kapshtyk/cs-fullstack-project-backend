using FluentValidation;
using Ecommerce.Domain.Models;
using Ecommerce.Services.Common.DTO;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.UserService.DTO
{
    public class CreateUserDto : ICreateDto<User>
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
        public IFormFile Avatar { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? AvatarPath { get; set; }

        public User GetEntity()
        {
            return new User
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Password = Password,
                Avatar = AvatarPath,
                Role = Role.User,
                CartItems = null!,
                Orders = null!,
                SaltUser = null!
            };
        }
    }

    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.FirstName).NotEmpty().MinimumLength(2).MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MinimumLength(2).MaximumLength(100);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(20);
        }
    }
}