using FriendWish.Core.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FriendWish.DataAccess.Repositories;

internal static class DbContextExtensions
{
    /// <summary>
    /// Сохраняет изменения, а нарушение уникального индекса превращает в <see cref="ConflictException"/>.
    /// Сервисы проверяют уникальность заранее, но два одновременных запроса эту проверку проходят оба —
    /// тогда последней защитой остаётся индекс.
    /// </summary>
    public static async Task SaveChangesOrConflictAsync(this DbContext context, CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        } postgresException)
        {
            // Несохранённая сущность иначе осталась бы в контексте и попала в следующее сохранение того же запроса.
            context.ChangeTracker.Clear();

            throw new ConflictException(UniqueConstraints.GetConflictMessage(postgresException.ConstraintName));
        }
    }
}
