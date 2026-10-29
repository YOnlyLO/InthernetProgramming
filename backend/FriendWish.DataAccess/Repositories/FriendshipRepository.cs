using FriendWish.Core.Common;
using FriendWish.Core.Enums;
using FriendWish.Core.Interfaces.Repositories;
using FriendWish.Core.Models;
using FriendWish.DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;

namespace FriendWish.DataAccess.Repositories;

public class FriendshipRepository(FriendWishDbContext context) : IFriendshipRepository
{
    public async Task<Friendship?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Friendships
            .AsNoTracking()
            .FirstOrDefaultAsync(friendship => friendship.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<Friendship?> GetBetweenAsync(
        Guid userId,
        Guid otherUserId,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Friendships
            .AsNoTracking()
            .FirstOrDefaultAsync(
                friendship => (friendship.RequesterId == userId && friendship.AddresseeId == otherUserId)
                    || (friendship.RequesterId == otherUserId && friendship.AddresseeId == userId),
                cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyList<Friendship>> GetByUserIdAsync(
        Guid userId,
        FriendshipStatus status,
        CancellationToken cancellationToken = default)
    {
        var entities = await context.Friendships
            .AsNoTracking()
            .Where(friendship => friendship.Status == status
                && (friendship.RequesterId == userId || friendship.AddresseeId == userId))
            .OrderByDescending(friendship => friendship.RequestedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(entity => entity.ToDomain()).ToList();
    }

    public async Task AddAsync(Friendship friendship, CancellationToken cancellationToken = default)
    {
        context.Friendships.Add(friendship.ToEntity());
        await context.SaveChangesOrConflictAsync(cancellationToken);
    }

    public async Task UpdateAsync(Friendship friendship, CancellationToken cancellationToken = default)
    {
        var entity = await context.Friendships.FindAsync([friendship.Id], cancellationToken)
            ?? throw new NotFoundException("Заявка не найдена.");

        friendship.CopyTo(entity);
        await context.SaveChangesOrConflictAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await context.Friendships
            .Where(friendship => friendship.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
