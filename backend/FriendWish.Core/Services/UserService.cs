using FriendWish.Core.Common;
using FriendWish.Core.Interfaces.Auth;
using FriendWish.Core.Interfaces.Repositories;
using FriendWish.Core.Interfaces.Services;
using FriendWish.Core.Models;

namespace FriendWish.Core.Services;

public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserService
{
    private const int SearchLimit = 20;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return userRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> SearchAsync(
        Guid userId,
        string query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        // Запрашиваем на одного больше, потому что самого пользователя из результата убираем.
        var found = await userRepository.SearchAsync(query.Trim(), SearchLimit + 1, cancellationToken);

        return found
            .Where(user => user.Id != userId && !user.IsBlocked)
            .Take(SearchLimit)
            .ToList();
    }

    public async Task<User> UpdateProfileAsync(
        Guid userId,
        string name,
        string? avatarUrl,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetRequiredAsync(userId, cancellationToken);

        user.ChangeName(name);
        user.ChangeAvatar(avatarUrl);

        await userRepository.UpdateAsync(user, cancellationToken);
        return user;
    }

    public async Task ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetRequiredAsync(userId, cancellationToken);

        // Обычная ошибка валидации, а не 401: на 401 клиент разлогинивает пользователя.
        if (string.IsNullOrEmpty(currentPassword) || !passwordHasher.Verify(currentPassword, user.PasswordHash))
            throw new DomainException("Текущий пароль указан неверно.");

        PasswordPolicy.Validate(newPassword);
        user.ChangePasswordHash(passwordHasher.Hash(newPassword));

        await userRepository.UpdateAsync(user, cancellationToken);
    }
}
