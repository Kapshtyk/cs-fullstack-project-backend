namespace Ecommerce.Controllers.CustomExceptions
{
    public class MethodNotAllowedException(string message = "Method not allowed") : Exception(message)
    { }
}