using System.Text.RegularExpressions;
using FriendWish.Core.Common;
using FriendWish.Core.Enums;

namespace FriendWish.Core.Models;

public partial class User
{
    public const int MaxNameLength = 100;
    public const int MinUserNameLength = 3;
    public const int MaxUserNameLength = 32;
    public const int MaxEmailLength = 254;
    public const int MaxAvatarUrlLength = 2048;

    private User(
        Guid id,
        string name,
        string userName,
        string email,
        string passwordHash,
        string? avatarUrl,
        UserRole role,
        UserStatus status,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        AvatarUrl = avatarUrl;
        Role = role;
        Status = status;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string UserName { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public string? AvatarUrl { get; private set; }

    public UserRole Role { get; private set; }

    public UserStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public bool IsBlocked => Status == UserStatus.Blocked;

    public bool IsAdmin => Role == UserRole.Admin;

    /// <summary>
    /// Создаёт нового пользователя с ролью <see cref="UserRole.User"/> и активной учётной записью.
    /// Пароль передаётся уже захэшированным: хэширование — ответственность слоя приложения.
    /// </summary>
    public static User Create(string name, string userName, string email, string passwordHash)
    {
        return new User(
            Guid.CreateVersion7(),
            ValidateName(name),
            ValidateUserName(userName),
            ValidateEmail(email),
            ValidatePasswordHash(passwordHash),
            avatarUrl: null,
            UserRole.User,
            UserStatus.Active,
            DateTime.UtcNow);
    }

    public void ChangeName(string name)
    {
        Name = ValidateName(name);
    }

    public void ChangeAvatar(string? avatarUrl)
    {
        AvatarUrl = Guard.Optional(avatarUrl, MaxAvatarUrlLength, "Аватар");
    }

    public void ChangePasswordHash(string passwordHash)
    {
        PasswordHash = ValidatePasswordHash(passwordHash);
    }

    public void ChangeRole(UserRole role)
    {
        Role = Guard.DefinedEnum(role, "Роль");
    }

    public void Block()
    {
        Status = UserStatus.Blocked;
    }

    public void Unblock()
    {
        Status = UserStatus.Active;
    }

    private static string ValidateName(string name)
    {
        return Guard.Required(name, MaxNameLength, "Имя");
    }

    private static string ValidateUserName(string userName)
    {
        var normalized = Guard.Required(userName, MaxUserNameLength, "Юзернейм").ToLowerInvariant();

        if (normalized.Length < MinUserNameLength)
            throw new DomainException(
                $"Длина юзернейма должна быть от {MinUserNameLength} до {MaxUserNameLength} симв.");

        if (!UserNameRegex().IsMatch(normalized))
            throw new DomainException("Юзернейм может содержать только латинские буквы, цифры и знак подчёркивания.");

        return normalized;
    }

    private static string ValidateEmail(string email)
    {
        var normalized = Guard.Required(email, MaxEmailLength, "Email").ToLowerInvariant();

        if (!EmailRegex().IsMatch(normalized))
            throw new DomainException("Некорректный адрес электронной почты.");

        return normalized;
    }

    private static string ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Хэш пароля не может быть пустым.");

        return passwordHash;
    }

    [GeneratedRegex("^[a-z0-9_]+$")]
    private static partial Regex UserNameRegex();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
