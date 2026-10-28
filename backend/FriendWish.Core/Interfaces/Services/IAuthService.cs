using FriendWish.Core.Models;

namespace FriendWish.Core.Interfaces.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(
        string name,
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет учётные данные и возвращает JWT. Заблокированный пользователь войти не может.
    /// </summary>
    Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
