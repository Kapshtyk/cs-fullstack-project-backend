namespace Ecommerce.Domain.Common.Exceptions
{
    public class ContstraintViolationException(string message) : Exception(message)
    { }
}