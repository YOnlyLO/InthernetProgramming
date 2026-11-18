using FriendWish.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendWish.DataAccess;

public class FriendWishDbContext(DbContextOptions<FriendWishDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    public DbSet<WishEntity> Wishes => Set<WishEntity>();

    public DbSet<FriendshipEntity> Friendships => Set<FriendshipEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Имена таблиц, колонок и индексов в snake_case, как принято в PostgreSQL: SQL пишется без кавычек.
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Перечисления храним строками: значения понятны при чтении БД и не зависят от порядка в enum.
        configurationBuilder.Properties<Enum>().HaveConversion<string>().HaveMaxLength(16);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FriendWishDbContext).Assembly);
    }
}
