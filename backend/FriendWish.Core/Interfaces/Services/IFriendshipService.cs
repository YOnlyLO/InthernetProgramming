using FriendWish.Core.Models;

namespace FriendWish.Core.Interfaces.Services;

/// <summary>
/// Друзья и заявки в друзья. <c>userId</c> — текущий пользователь из JWT.
/// </summary>
public interface IFriendshipService
{
    Task<IReadOnlyList<User>> GetFriendsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FriendRequest>> GetIncomingRequestsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FriendRequest>> GetOutgoingRequestsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет заявку. Если ранее заявка между пользователями была отклонена, отправляет её повторно.
    /// </summary>
    Task<Friendship> SendRequestAsync(Guid userId, Guid addresseeId, CancellationToken cancellationToken = default);

    Task AcceptRequestAsync(Guid userId, Guid requestId, CancellationToken cancellationToken = default);

    Task DeclineRequestAsync(Guid userId, Guid requestId, CancellationToken cancellationToken = default);

    Task RemoveFriendAsync(Guid userId, Guid friendId, CancellationToken cancellationToken = default);
}
