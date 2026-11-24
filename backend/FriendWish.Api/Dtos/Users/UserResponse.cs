namespace FriendWish.Api.Dtos.Users;

/// <summary>
/// Публичная информация о пользователе: для поиска, списка друзей и заявок. Email и роль сюда не входят.
/// </summary>
public sealed record UserResponse(Guid Id, string Name, string UserName, string? AvatarUrl);
