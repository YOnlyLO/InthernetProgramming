using FriendWish.Core.Common;
using FriendWish.Core.Enums;
using FriendWish.Core.Interfaces.Repositories;
using FriendWish.Core.Interfaces.Services;
using FriendWish.Core.Models;

namespace FriendWish.Core.Services;

public class AdminService(IUserRepository userRepository) : IAdminService
{
    public Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return userRepository.GetAllAsync(cancellationToken);
    }

    public async Task ChangeRoleAsync(
        Guid adminId,
        Guid userId,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        if (adminId == userId)
            throw new DomainException("Нельзя изменить собственную роль.");

        var user = await userRepository.GetRequiredAsync(userId, cancellationToken);

        user.ChangeRole(role);

        await userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task BlockAsync(Guid adminId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (adminId == userId)
            throw new DomainException("Нельзя заблокировать самого себя.");

        var user = await userRepository.GetRequiredAsync(userId, cancellationToken);

        user.Block();

        await userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task UnblockAsync(Guid adminId, Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetRequiredAsync(userId, cancellationToken);

        user.Unblock();

        await userRepository.UpdateAsync(user, cancellationToken);
    }
}
