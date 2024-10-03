using FluentValidation;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Services.AuthService.DTO
{
    public class RegisterCredentialsDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required IFormFile Avatar { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? AvatarPath { get; set; }


        public class RegisterCredentialsDtoValidator : AbstractValidator<RegisterCredentialsDto>
        {
            public RegisterCredentialsDtoValidator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(256);
                RuleFor(x => x.FirstName).NotEmpty().MinimumLength(2).MaximumLength(100);
                RuleFor(x => x.LastName).NotEmpty().MinimumLength(2).MaximumLength(100);
                RuleFor(x => x.Avatar).NotNull();
            }
        }
    }
}