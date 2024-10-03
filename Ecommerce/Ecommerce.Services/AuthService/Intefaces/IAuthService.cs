
using Ecommerce.Services.AuthService.DTO;

namespace Ecommerce.Services.AuthService.Intefaces
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(LoginCredentialsDto userCredentials);
        Task<LoginResultDto> RefreshTokenAsync(string token);
        Task<bool> LogoutAsync(string token);
    }
}