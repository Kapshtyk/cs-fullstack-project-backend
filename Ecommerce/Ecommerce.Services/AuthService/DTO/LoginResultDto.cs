namespace Ecommerce.Services.AuthService.DTO
{
    public class LoginResultDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}