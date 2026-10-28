using FriendWish.Core.Enums;
using FriendWish.Core.Models;

namespace FriendWish.Core.Interfaces.Services;

/// <summary>
/// Административное управление пользователями. <c>adminId</c> — текущий администратор из JWT:
/// по нему сервис не даёт администратору заблокировать себя или снять с себя роль.
/// </summary>
public interface IAdminService
{
    Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task ChangeRoleAsync(Guid adminId, Guid userId, UserRole role, CancellationToken cancellationToken = default);

    Task BlockAsync(Guid adminId, Guid userId, CancellationToken cancellationToken = default);

    Task UnblockAsync(Guid adminId, Guid userId, CancellationToken cancellationToken = default);
}
