using System.ComponentModel.DataAnnotations;

namespace FriendWish.Api.Dtos.Auth;

/// <summary>
/// Данные для регистрации. Здесь проверяются только обязательность полей и совпадение паролей,
/// формат email, длины и допустимые символы проверяет доменная модель.
/// </summary>
public sealed record RegisterRequest
{
    /// <summary>Имя, которое видят другие пользователи.</summary>
    [Required(ErrorMessage = "Поле «Имя» обязательно для заполнения.")]
    public string Name { get; init; } = string.Empty;

    /// <summary>Уникальный юзернейм: латинские буквы, цифры и знак подчёркивания.</summary>
    [Required(ErrorMessage = "Поле «Юзернейм» обязательно для заполнения.")]
    public string UserName { get; init; } = string.Empty;

    /// <summary>Email, по которому пользователь входит в систему.</summary>
    [Required(ErrorMessage = "Поле «Email» обязательно для заполнения.")]
    public string Email { get; init; } = string.Empty;

    /// <summary>Пароль от 8 до 128 символов.</summary>
    [Required(ErrorMessage = "Поле «Пароль» обязательно для заполнения.")]
    public string Password { get; init; } = string.Empty;

    /// <summary>Повтор пароля, должен совпадать с <see cref="Password"/>.</summary>
    [Required(ErrorMessage = "Поле «Подтверждение пароля» обязательно для заполнения.")]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; init; } = string.Empty;
}
