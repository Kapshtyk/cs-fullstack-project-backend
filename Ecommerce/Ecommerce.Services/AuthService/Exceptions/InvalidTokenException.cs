namespace Ecommerce.Services.AuthService.Exceptions
{
    public class InvalidTokenException(string message = "Invalid token") : Exception(message)
    { }
}