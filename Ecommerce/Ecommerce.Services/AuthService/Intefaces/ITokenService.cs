namespace Ecommerce.Services.AuthService.Intefaces
{
    public interface ITokenService
    {
        string GenerateToken(TokenOptions tokenOptions, bool isRefreshToken = false);
        TokenOptions DecodeToken(string token);
        bool ValidateToken(string token);
    }
}