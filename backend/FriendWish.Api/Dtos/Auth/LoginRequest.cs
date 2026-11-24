using System.ComponentModel.DataAnnotations;

namespace FriendWish.Api.Dtos.Auth;

/// <summary>
/// Учётные данные для входа.
/// </summary>
public sealed record LoginRequest
{
    [Required(ErrorMessage = "Поле «Email» обязательно для заполнения.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Поле «Пароль» обязательно для заполнения.")]
    public string Password { get; init; } = string.Empty;
}
