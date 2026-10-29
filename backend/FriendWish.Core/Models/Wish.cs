using FriendWish.Core.Common;

namespace FriendWish.Core.Models;

public class Wish
{
    public const int MaxTitleLength = 200;
    public const int MaxDescriptionLength = 1000;
    public const int MaxUrlLength = 2048;

    private Wish(
        Guid id,
        Guid userId,
        string title,
        string? description,
        string? link,
        decimal? price,
        string? imageUrl,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Description = description;
        Link = link;
        Price = price;
        ImageUrl = imageUrl;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public string? Link { get; private set; }

    public decimal? Price { get; private set; }

    public string? ImageUrl { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static Wish Create(
        Guid userId,
        string title,
        string? description,
        string? link,
        decimal? price,
        string? imageUrl)
    {
        var now = DateTime.UtcNow;

        return new Wish(
            Guid.CreateVersion7(),
            Guard.NotEmpty(userId, "Владелец"),
            ValidateTitle(title),
            ValidateDescription(description),
            ValidateUrl(link, "Ссылка"),
            ValidatePrice(price),
            ValidateUrl(imageUrl, "Изображение"),
            now,
            now);
    }

    /// <summary>
    /// Восстанавливает сохранённое желание без проверок: данные проверены при создании и изменении.
    /// Только для слоя доступа к данным — новое желание создаёт <see cref="Create"/>.
    /// </summary>
    public static Wish Restore(
        Guid id,
        Guid userId,
        string title,
        string? description,
        string? link,
        decimal? price,
        string? imageUrl,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Wish(id, userId, title, description, link, price, imageUrl, createdAt, updatedAt);
    }

    public void Update(string title, string? description, string? link, decimal? price, string? imageUrl)
    {
        // Сначала проверяем все значения, чтобы при ошибке не оставить желание изменённым наполовину.
        var validTitle = ValidateTitle(title);
        var validDescription = ValidateDescription(description);
        var validLink = ValidateUrl(link, "Ссылка");
        var validPrice = ValidatePrice(price);
        var validImageUrl = ValidateUrl(imageUrl, "Изображение");

        Title = validTitle;
        Description = validDescription;
        Link = validLink;
        Price = validPrice;
        ImageUrl = validImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsOwnedBy(Guid userId) => UserId == userId;

    private static string ValidateTitle(string title)
    {
        return Guard.Required(title, MaxTitleLength, "Название");
    }

    private static string? ValidateDescription(string? description)
    {
        return Guard.Optional(description, MaxDescriptionLength, "Описание");
    }

    private static string? ValidateUrl(string? url, string fieldName)
    {
        return Guard.OptionalUrl(url, MaxUrlLength, fieldName);
    }

    private static decimal? ValidatePrice(decimal? price)
    {
        if (price is null)
            return null;

        if (price < 0)
            throw new DomainException("Цена не может быть отрицательной.");

        if (decimal.Round(price.Value, 2) != price.Value)
            throw new DomainException("Цена может содержать не более двух знаков после запятой.");

        return price;
    }
}
