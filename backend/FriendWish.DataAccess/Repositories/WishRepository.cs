using FriendWish.Core.Common;
using FriendWish.Core.Interfaces.Repositories;
using FriendWish.Core.Models;
using FriendWish.DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;

namespace FriendWish.DataAccess.Repositories;

public class WishRepository(FriendWishDbContext context) : IWishRepository
{
    public async Task<Wish?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Wishes
            .AsNoTracking()
            .FirstOrDefaultAsync(wish => wish.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyList<Wish>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var entities = await context.Wishes
            .AsNoTracking()
            .Where(wish => wish.UserId == userId)
            .OrderByDescending(wish => wish.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(entity => entity.ToDomain()).ToList();
    }

    public async Task AddAsync(Wish wish, CancellationToken cancellationToken = default)
    {
        context.Wishes.Add(wish.ToEntity());
        await context.SaveChangesOrConflictAsync(cancellationToken);
    }

    public async Task UpdateAsync(Wish wish, CancellationToken cancellationToken = default)
    {
        var entity = await context.Wishes.FindAsync([wish.Id], cancellationToken)
            ?? throw new NotFoundException("Желание не найдено.");

        wish.CopyTo(entity);
        await context.SaveChangesOrConflictAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await context.Wishes
            .Where(wish => wish.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
