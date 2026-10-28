namespace FriendWish.Core.Models;

/// <summary>
/// Заявка в друзья вместе со вторым участником: для входящей заявки это отправитель, для исходящей — получатель.
/// </summary>
public record FriendRequest(Friendship Friendship, User User);
