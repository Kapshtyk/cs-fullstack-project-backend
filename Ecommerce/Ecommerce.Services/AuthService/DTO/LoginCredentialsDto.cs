using FluentValidation;

namespace Ecommerce.Services.AuthService.DTO
{
    public class LoginCredentialsDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }


        public class LoginCredentialsDtoValidator : AbstractValidator<LoginCredentialsDto>
        {
            public LoginCredentialsDtoValidator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(256);
            }
        }
    }
}