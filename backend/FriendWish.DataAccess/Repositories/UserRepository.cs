using FriendWish.Core.Common;
using FriendWish.Core.Interfaces.Repositories;
using FriendWish.Core.Models;
using FriendWish.DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;

namespace FriendWish.DataAccess.Repositories;

public class UserRepository(FriendWishDbContext context) : IUserRepository
{
    private const string LikeEscapeCharacter = @"\";

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyList<User>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
            return [];

        var entities = await context.Users
            .AsNoTracking()
            .Where(user => ids.Contains(user.Id))
            .ToListAsync(cancellationToken);

        return entities.Select(entity => entity.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.Users
            .AsNoTracking()
            .OrderBy(user => user.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(entity => entity.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<User>> SearchAsync(
        string query,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var pattern = $"%{EscapeLikePattern(query)}%";

        var entities = await context.Users
            .AsNoTracking()
            .Where(user => EF.Functions.ILike(user.Name, pattern, LikeEscapeCharacter)
                || EF.Functions.ILike(user.UserName, pattern, LikeEscapeCharacter))
            .OrderBy(user => user.Name)
            .ThenBy(user => user.UserName)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return entities.Select(entity => entity.ToDomain()).ToList();
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return context.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return context.Users.AnyAsync(user => user.UserName == userName, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(user.ToEntity());
        await context.SaveChangesOrConflictAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await context.Users.FindAsync([user.Id], cancellationToken)
            ?? throw new NotFoundException("Пользователь не найден.");

        user.CopyTo(entity);
        await context.SaveChangesOrConflictAsync(cancellationToken);
    }

    /// <summary>
    /// Экранирует «%» и «_»: иначе они работали бы в ILIKE как подстановочные знаки и запрос «_» нашёл бы всех.
    /// </summary>
    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace(LikeEscapeCharacter, LikeEscapeCharacter + LikeEscapeCharacter)
            .Replace("%", LikeEscapeCharacter + "%")
            .Replace("_", LikeEscapeCharacter + "_");
    }
}
