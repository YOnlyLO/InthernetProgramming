using FriendWish.Core.Models;
using FriendWish.DataAccess.Entities;

namespace FriendWish.DataAccess.Mappers;

internal static class UserMapper
{
    public static User ToDomain(this UserEntity entity)
    {
        return User.Restore(
            entity.Id,
            entity.Name,
            entity.UserName,
            entity.Email,
            entity.PasswordHash,
            entity.AvatarUrl,
            entity.Role,
            entity.Status,
            entity.CreatedAt);
    }

    public static UserEntity ToEntity(this User user)
    {
        var entity = new UserEntity { Id = user.Id };
        user.CopyTo(entity);

        return entity;
    }

    /// <summary>
    /// Переносит поля модели в сущность. Для отслеживаемой сущности EF Core сохранит только изменившиеся поля.
    /// </summary>
    public static void CopyTo(this User user, UserEntity entity)
    {
        entity.Name = user.Name;
        entity.UserName = user.UserName;
        entity.Email = user.Email;
        entity.PasswordHash = user.PasswordHash;
        entity.AvatarUrl = user.AvatarUrl;
        entity.Role = user.Role;
        entity.Status = user.Status;
        entity.CreatedAt = user.CreatedAt;
    }
}
