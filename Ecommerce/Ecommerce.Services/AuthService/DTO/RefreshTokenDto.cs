using FluentValidation;

namespace Ecommerce.Services.AuthService.DTO
{
    public class RefreshTokenDto
    {
        public required string RefreshToken { get; set; }

        public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
        {
            public RefreshTokenDtoValidator()
            {
                RuleFor(x => x.RefreshToken).NotEmpty();
            }
        }
    }
}