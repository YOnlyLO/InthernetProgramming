using System.ComponentModel.DataAnnotations;

namespace FriendWish.Api.Dtos.Wishes;

/// <summary>
/// Данные желания — для добавления и для изменения. Обязательно только название.
/// </summary>
public sealed record WishRequest
{
    [Required(ErrorMessage = "Поле «Название» обязательно для заполнения.")]
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    /// <summary>Ссылка на товар (http или https).</summary>
    public string? Link { get; init; }

    /// <summary>Цена, не больше двух знаков после запятой.</summary>
    public decimal? Price { get; init; }

    /// <summary>Ссылка на изображение товара (http или https).</summary>
    public string? ImageUrl { get; init; }
}
