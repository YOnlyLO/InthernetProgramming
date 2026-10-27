using FriendWish.Core.Common;
using FriendWish.Core.Interfaces.Repositories;
using FriendWish.Core.Interfaces.Services;
using FriendWish.Core.Models;

namespace FriendWish.Core.Services;

public class WishService(IWishRepository wishRepository, IFriendshipRepository friendshipRepository) : IWishService
{
    public Task<IReadOnlyList<Wish>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return wishRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Wish>> GetFriendWishesAsync(
        Guid userId,
        Guid friendId,
        CancellationToken cancellationToken = default)
    {
        if (userId != friendId)
        {
            var friendship = await friendshipRepository.GetBetweenAsync(userId, friendId, cancellationToken);
            if (friendship is not { IsAccepted: true })
                throw new ForbiddenException("Список желаний доступен только друзьям.");
        }

        return await wishRepository.GetByUserIdAsync(friendId, cancellationToken);
    }

    public async Task<Wish> CreateAsync(
        Guid userId,
        string title,
        string? description,
        string? link,
        decimal? price,
        string? imageUrl,
        CancellationToken cancellationToken = default)
    {
        var wish = Wish.Create(userId, title, description, link, price, imageUrl);

        await wishRepository.AddAsync(wish, cancellationToken);
        return wish;
    }

    public async Task<Wish> UpdateAsync(
        Guid userId,
        Guid wishId,
        string title,
        string? description,
        string? link,
        decimal? price,
        string? imageUrl,
        CancellationToken cancellationToken = default)
    {
        var wish = await GetOwnWishAsync(userId, wishId, cancellationToken);

        wish.Update(title, description, link, price, imageUrl);

        await wishRepository.UpdateAsync(wish, cancellationToken);
        return wish;
    }

    public async Task DeleteAsync(Guid userId, Guid wishId, CancellationToken cancellationToken = default)
    {
        var wish = await GetOwnWishAsync(userId, wishId, cancellationToken);

        await wishRepository.DeleteAsync(wish.Id, cancellationToken);
    }

    private async Task<Wish> GetOwnWishAsync(Guid userId, Guid wishId, CancellationToken cancellationToken)
    {
        var wish = await wishRepository.GetByIdAsync(wishId, cancellationToken)
            ?? throw new NotFoundException("Желание не найдено.");

        if (!wish.IsOwnedBy(userId))
            throw new ForbiddenException("Изменять и удалять можно только свои желания.");

        return wish;
    }
}
