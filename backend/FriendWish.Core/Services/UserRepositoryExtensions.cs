using FriendWish.Core.Common;
using FriendWish.Core.Interfaces.Repositories;
using FriendWish.Core.Models;

namespace FriendWish.Core.Services;

internal static class UserRepositoryExtensions
{
    public static async Task<User> GetRequiredAsync(
        this IUserRepository userRepository,
        Guid id,
        CancellationToken cancellationToken)
    {
        return await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Пользователь не найден.");
    }
}
