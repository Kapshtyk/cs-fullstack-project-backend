using Ecommerce.Domain.Common;

namespace Ecommerce.Services.Common.DTO
{

    public interface IReadDto<T> where T : BaseEntity
    {
        public int Id { get; set; }

        void FromEntity(T entity);
    }

    public interface ICreateDto<T> where T : BaseEntity
    {
        T GetEntity();
    }

    public interface IPartialUpdateDto<T> where T : BaseEntity
    {
        T GetUpdatedEntity(T entity);
    }

    public interface IUpdateDto<T> where T : BaseEntity
    {
        T GetUpdatedEntity(T entity);
    }
}