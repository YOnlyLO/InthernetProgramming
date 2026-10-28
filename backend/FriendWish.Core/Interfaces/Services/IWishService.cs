using FriendWish.Core.Models;

namespace FriendWish.Core.Interfaces.Services;

/// <summary>
/// Список желаний. <c>userId</c> — текущий пользователь из JWT: менять и удалять можно только свои желания.
/// </summary>
public interface IWishService
{
    Task<IReadOnlyList<Wish>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список желаний друга; доступен только при принятой заявке в друзья.
    /// </summary>
    Task<IReadOnlyList<Wish>> GetFriendWishesAsync(
        Guid userId,
        Guid friendId,
        CancellationToken cancellationToken = default);

    Task<Wish> CreateAsync(
        Guid userId,
        string title,
        string? description,
        string? link,
        decimal? price,
        string? imageUrl,
        CancellationToken cancellationToken = default);

    Task<Wish> UpdateAsync(
        Guid userId,
        Guid wishId,
        string title,
        string? description,
        string? link,
        decimal? price,
        string? imageUrl,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, Guid wishId, CancellationToken cancellationToken = default);
}
