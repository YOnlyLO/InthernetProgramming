using FriendWish.Core.Models;

namespace FriendWish.Core.Interfaces.Services;

/// <summary>
/// Профиль и поиск пользователей. <c>userId</c> — текущий пользователь из JWT.
/// </summary>
public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ищет пользователей по имени и юзернейму, не включая в результат самого пользователя.
    /// </summary>
    Task<IReadOnlyList<User>> SearchAsync(Guid userId, string query, CancellationToken cancellationToken = default);

    Task<User> UpdateProfileAsync(
        Guid userId,
        string name,
        string? avatarUrl,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}
