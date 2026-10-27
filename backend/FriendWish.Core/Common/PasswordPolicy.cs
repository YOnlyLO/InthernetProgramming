namespace FriendWish.Core.Common;

/// <summary>
/// Требования к паролю. Проверяется исходный пароль до хэширования, поэтому правило живёт отдельно от <c>User</c>,
/// который хранит только хэш.
/// </summary>
public static class PasswordPolicy
{
    public const int MinLength = 8;
    public const int MaxLength = 128;

    public static void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Поле «Пароль» обязательно для заполнения.");

        if (password.Length < MinLength || password.Length > MaxLength)
            throw new DomainException($"Длина пароля должна быть от {MinLength} до {MaxLength} симв.");
    }
}
