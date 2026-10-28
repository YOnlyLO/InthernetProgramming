using FriendWish.Core.Enums;
using FriendWish.Core.Models;

namespace FriendWish.Core.Interfaces.Repositories;

public interface IFriendshipRepository
{
    Task<Friendship?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает запись между двумя пользователями независимо от того, кто отправил заявку.
    /// </summary>
    Task<Friendship?> GetBetweenAsync(Guid userId, Guid otherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает записи с указанным статусом, в которых пользователь — отправитель или получатель.
    /// </summary>
    Task<IReadOnlyList<Friendship>> GetByUserIdAsync(
        Guid userId,
        FriendshipStatus status,
        CancellationToken cancellationToken = default);

    Task AddAsync(Friendship friendship, CancellationToken cancellationToken = default);

    Task UpdateAsync(Friendship friendship, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
