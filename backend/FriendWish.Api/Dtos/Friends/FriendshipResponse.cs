using FriendWish.Core.Enums;

namespace FriendWish.Api.Dtos.Friends;

/// <summary>
/// Заявка в друзья после отправки. Если второй пользователь уже позвал в друзья, заявка сразу принята —
/// тогда <see cref="Status"/> равен <c>Accepted</c>.
/// </summary>
public sealed record FriendshipResponse(
    Guid Id,
    Guid RequesterId,
    Guid AddresseeId,
    FriendshipStatus Status,
    DateTime RequestedAt,
    DateTime? RespondedAt);
