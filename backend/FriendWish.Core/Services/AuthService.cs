using FriendWish.Core.Common;
using FriendWish.Core.Interfaces.Auth;
using FriendWish.Core.Interfaces.Repositories;
using FriendWish.Core.Interfaces.Services;
using FriendWish.Core.Models;

namespace FriendWish.Core.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider) : IAuthService
{
    public async Task<User> RegisterAsync(
        string name,
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        PasswordPolicy.Validate(password);

        // Уникальность проверяем по уже нормализованным значениям из модели.
        var user = User.Create(name, userName, email, passwordHasher.Hash(password));

        if (await userRepository.ExistsByEmailAsync(user.Email, cancellationToken))
            throw new ConflictException("Пользователь с таким email уже зарегистрирован.");

        if (await userRepository.ExistsByUserNameAsync(user.UserName, cancellationToken))
            throw new ConflictException("Этот юзернейм уже занят.");

        await userRepository.AddAsync(user, cancellationToken);
        return user;
    }

    public async Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            throw new InvalidCredentialsException();

        var user = await userRepository.GetByEmailAsync(User.NormalizeEmail(email), cancellationToken);
        if (user is null || !passwordHasher.Verify(password, user.PasswordHash))
            throw new InvalidCredentialsException();

        // Блокировку проверяем после пароля, чтобы не раскрывать статус учётной записи без верных данных.
        if (user.IsBlocked)
            throw new ForbiddenException("Учётная запись заблокирована.");

        return jwtProvider.GenerateToken(user);
    }
}
