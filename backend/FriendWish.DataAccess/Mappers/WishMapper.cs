using FriendWish.Core.Models;
using FriendWish.DataAccess.Entities;

namespace FriendWish.DataAccess.Mappers;

internal static class WishMapper
{
    public static Wish ToDomain(this WishEntity entity)
    {
        return Wish.Restore(
            entity.Id,
            entity.UserId,
            entity.Title,
            entity.Description,
            entity.Link,
            entity.Price,
            entity.ImageUrl,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static WishEntity ToEntity(this Wish wish)
    {
        var entity = new WishEntity { Id = wish.Id };
        wish.CopyTo(entity);

        return entity;
    }

    /// <summary>
    /// Переносит поля модели в сущность. Для отслеживаемой сущности EF Core сохранит только изменившиеся поля.
    /// </summary>
    public static void CopyTo(this Wish wish, WishEntity entity)
    {
        entity.UserId = wish.UserId;
        entity.Title = wish.Title;
        entity.Description = wish.Description;
        entity.Link = wish.Link;
        entity.Price = wish.Price;
        entity.ImageUrl = wish.ImageUrl;
        entity.CreatedAt = wish.CreatedAt;
        entity.UpdatedAt = wish.UpdatedAt;
    }
}
