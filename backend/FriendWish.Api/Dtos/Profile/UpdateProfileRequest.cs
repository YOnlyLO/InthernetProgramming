using System.ComponentModel.DataAnnotations;

namespace FriendWish.Api.Dtos.Profile;

/// <summary>
/// Новые данные профиля. Юзернейм и email не меняются.
/// </summary>
public sealed record UpdateProfileRequest
{
    [Required(ErrorMessage = "Поле «Имя» обязательно для заполнения.")]
    public string Name { get; init; } = string.Empty;

    /// <summary>Ссылка на аватар; пустое значение убирает аватар.</summary>
    public string? AvatarUrl { get; init; }
}
