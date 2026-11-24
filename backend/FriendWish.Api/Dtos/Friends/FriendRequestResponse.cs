using FriendWish.Api.Dtos.Users;

namespace FriendWish.Api.Dtos.Friends;

/// <summary>
/// Заявка в друзья, ожидающая ответа. <c>user</c> — второй участник: для входящей заявки это отправитель,
/// для исходящей — получатель.
/// </summary>
public sealed record FriendRequestResponse(Guid Id, UserResponse User, DateTime RequestedAt);
