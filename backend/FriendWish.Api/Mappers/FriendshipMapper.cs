using FriendWish.Api.Dtos.Friends;
using FriendWish.Core.Models;

namespace FriendWish.Api.Mappers;

internal static class FriendshipMapper
{
    public static FriendshipResponse ToResponse(this Friendship friendship)
    {
        return new FriendshipResponse(
            friendship.Id,
            friendship.RequesterId,
            friendship.AddresseeId,
            friendship.Status,
            friendship.RequestedAt,
            friendship.RespondedAt);
    }

    public static FriendRequestResponse ToResponse(this FriendRequest request)
    {
        return new FriendRequestResponse(
            request.Friendship.Id,
            request.User.ToUserResponse(),
            request.Friendship.RequestedAt);
    }
}
