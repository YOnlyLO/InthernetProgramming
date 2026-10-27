using FriendWish.Core.Models;

namespace FriendWish.Core.Interfaces.Repositories;

public interface IWishRepository
{
    Task<Wish?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Wish>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Wish wish, CancellationToken cancellationToken = default);

    Task UpdateAsync(Wish wish, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
