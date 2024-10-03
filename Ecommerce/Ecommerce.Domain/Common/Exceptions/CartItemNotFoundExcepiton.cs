namespace Ecommerce.Domain.Common.Exceptions
{
    public class CartItemNotFoundException(string message = "Cart item not found") : Exception(message)
    { }
}