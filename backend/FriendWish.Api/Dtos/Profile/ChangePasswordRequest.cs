using System.ComponentModel.DataAnnotations;

namespace FriendWish.Api.Dtos.Profile;

/// <summary>
/// Смена пароля с проверкой текущего.
/// </summary>
public sealed record ChangePasswordRequest
{
    [Required(ErrorMessage = "Поле «Текущий пароль» обязательно для заполнения.")]
    public string CurrentPassword { get; init; } = string.Empty;

    /// <summary>Новый пароль от 8 до 128 символов.</summary>
    [Required(ErrorMessage = "Поле «Новый пароль» обязательно для заполнения.")]
    public string NewPassword { get; init; } = string.Empty;

    /// <summary>Повтор нового пароля, должен совпадать с <c>newPassword</c>.</summary>
    [Required(ErrorMessage = "Поле «Подтверждение пароля» обязательно для заполнения.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmNewPassword { get; init; } = string.Empty;
}
