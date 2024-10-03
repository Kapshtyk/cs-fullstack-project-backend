namespace Ecommerce.Domain.Common.Exceptions
{
    public class EntityNotFoundException<T> : Exception where T : BaseEntity
    {
        public EntityNotFoundException(string message) : base(message)
        { }

        public EntityNotFoundException() : base($"{typeof(T).Name} not found")
        { }
    }
}