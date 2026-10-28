using FriendWish.Core.Common;
using FriendWish.Core.Enums;
using FriendWish.Core.Interfaces.Repositories;
using FriendWish.Core.Interfaces.Services;
using FriendWish.Core.Models;

namespace FriendWish.Core.Services;

public class FriendshipService(
    IFriendshipRepository friendshipRepository,
    IUserRepository userRepository) : IFriendshipService
{
    public async Task<IReadOnlyList<User>> GetFriendsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var friendships = await friendshipRepository.GetByUserIdAsync(
            userId,
            FriendshipStatus.Accepted,
            cancellationToken);

        if (friendships.Count == 0)
            return [];

        var friendIds = friendships.Select(friendship => friendship.GetOtherUserId(userId)).ToList();
        var friends = await userRepository.GetByIdsAsync(friendIds, cancellationToken);

        return friends.OrderBy(friend => friend.Name).ToList();
    }

    public async Task<IReadOnlyList<FriendRequest>> GetIncomingRequestsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var pending = await friendshipRepository.GetByUserIdAsync(userId, FriendshipStatus.Pending, cancellationToken);

        return await ToFriendRequestsAsync(
            userId,
            pending.Where(request => request.AddresseeId == userId),
            cancellationToken);
    }

    public async Task<IReadOnlyList<FriendRequest>> GetOutgoingRequestsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var pending = await friendshipRepository.GetByUserIdAsync(userId, FriendshipStatus.Pending, cancellationToken);

        return await ToFriendRequestsAsync(
            userId,
            pending.Where(request => request.RequesterId == userId),
            cancellationToken);
    }

    public async Task<Friendship> SendRequestAsync(
        Guid userId,
        Guid addresseeId,
        CancellationToken cancellationToken = default)
    {
        await userRepository.GetRequiredAsync(addresseeId, cancellationToken);

        var friendship = await friendshipRepository.GetBetweenAsync(userId, addresseeId, cancellationToken);
        if (friendship is null)
        {
            friendship = Friendship.Create(userId, addresseeId);
            await friendshipRepository.AddAsync(friendship, cancellationToken);
            return friendship;
        }

        switch (friendship.Status)
        {
            case FriendshipStatus.Accepted:
                throw new ConflictException("Пользователь уже у вас в друзьях.");

            case FriendshipStatus.Pending when friendship.RequesterId == userId:
                throw new ConflictException("Заявка уже отправлена.");

            case FriendshipStatus.Pending:
                // Встречная заявка: второй пользователь уже позвал в друзья, значит, согласны оба.
                friendship.Accept(userId);
                break;

            case FriendshipStatus.Declined:
                friendship.Resend(userId);
                break;
        }

        await friendshipRepository.UpdateAsync(friendship, cancellationToken);
        return friendship;
    }

    public async Task AcceptRequestAsync(Guid userId, Guid requestId, CancellationToken cancellationToken = default)
    {
        var friendship = await GetUserRequestAsync(userId, requestId, cancellationToken);

        friendship.Accept(userId);

        await friendshipRepository.UpdateAsync(friendship, cancellationToken);
    }

    public async Task DeclineRequestAsync(Guid userId, Guid requestId, CancellationToken cancellationToken = default)
    {
        var friendship = await GetUserRequestAsync(userId, requestId, cancellationToken);

        friendship.Decline(userId);

        await friendshipRepository.UpdateAsync(friendship, cancellationToken);
    }

    public async Task RemoveFriendAsync(Guid userId, Guid friendId, CancellationToken cancellationToken = default)
    {
        var friendship = await friendshipRepository.GetBetweenAsync(userId, friendId, cancellationToken);
        if (friendship is not { IsAccepted: true })
            throw new NotFoundException("Пользователь не найден в списке друзей.");

        await friendshipRepository.DeleteAsync(friendship.Id, cancellationToken);
    }

    private async Task<Friendship> GetUserRequestAsync(Guid userId, Guid requestId, CancellationToken cancellationToken)
    {
        var friendship = await friendshipRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new NotFoundException("Заявка не найдена.");

        if (!friendship.Involves(userId))
            throw new ForbiddenException("Нет доступа к этой заявке.");

        return friendship;
    }

    private async Task<IReadOnlyList<FriendRequest>> ToFriendRequestsAsync(
        Guid userId,
        IEnumerable<Friendship> requests,
        CancellationToken cancellationToken)
    {
        var sortedRequests = requests.OrderByDescending(request => request.RequestedAt).ToList();
        if (sortedRequests.Count == 0)
            return [];

        var otherUserIds = sortedRequests.Select(request => request.GetOtherUserId(userId)).ToList();
        var usersById = (await userRepository.GetByIdsAsync(otherUserIds, cancellationToken))
            .ToDictionary(user => user.Id);

        return sortedRequests
            .Select(request => new FriendRequest(request, usersById[request.GetOtherUserId(userId)]))
            .ToList();
    }
}
