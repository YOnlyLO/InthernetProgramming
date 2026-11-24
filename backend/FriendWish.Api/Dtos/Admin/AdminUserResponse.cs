using FriendWish.Core.Enums;

namespace FriendWish.Api.Dtos.Admin;

/// <summary>
/// Учётная запись в административной панели.
/// </summary>
public sealed record AdminUserResponse(
    Guid Id,
    string Name,
    string UserName,
    string Email,
    UserRole Role,
    UserStatus Status,
    DateTime CreatedAt);
