namespace FriendWish.Api.Dtos.Friends;

/// <summary>
/// Входящие и исходящие заявки, ожидающие ответа, — новые первыми.
/// </summary>
public sealed record FriendRequestsResponse(
    IReadOnlyList<FriendRequestResponse> Incoming,
    IReadOnlyList<FriendRequestResponse> Outgoing);
