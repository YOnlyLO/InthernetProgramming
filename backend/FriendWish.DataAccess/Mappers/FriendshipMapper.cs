using FriendWish.Core.Models;
using FriendWish.DataAccess.Entities;

namespace FriendWish.DataAccess.Mappers;

internal static class FriendshipMapper
{
    public static Friendship ToDomain(this FriendshipEntity entity)
    {
        return Friendship.Restore(
            entity.Id,
            entity.RequesterId,
            entity.AddresseeId,
            entity.Status,
            entity.RequestedAt,
            entity.RespondedAt);
    }

    public static FriendshipEntity ToEntity(this Friendship friendship)
    {
        var entity = new FriendshipEntity { Id = friendship.Id };
        friendship.CopyTo(entity);

        return entity;
    }

    /// <summary>
    /// Переносит поля модели в сущность. Для отслеживаемой сущности EF Core сохранит только изменившиеся поля.
    /// Отправитель и получатель тоже переносятся: повторная заявка может поменять их местами.
    /// </summary>
    public static void CopyTo(this Friendship friendship, FriendshipEntity entity)
    {
        entity.RequesterId = friendship.RequesterId;
        entity.AddresseeId = friendship.AddresseeId;
        entity.Status = friendship.Status;
        entity.RequestedAt = friendship.RequestedAt;
        entity.RespondedAt = friendship.RespondedAt;
    }
}
