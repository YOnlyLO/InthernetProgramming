using FriendWish.Core.Enums;

namespace FriendWish.Api.Dtos.Profile;

/// <summary>
/// Собственный профиль пользователя. В отличие от публичного профиля, содержит email и роль.
/// </summary>
public sealed record ProfileResponse(
    Guid Id,
    string Name,
    string UserName,
    string Email,
    string? AvatarUrl,
    UserRole Role,
    DateTime CreatedAt);
