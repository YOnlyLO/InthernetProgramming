using FriendWish.Core.Interfaces.Repositories;
using FriendWish.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FriendWish.DataAccess;

public static class DependencyInjection
{
    /// <summary>
    /// Регистрирует контекст БД на PostgreSQL и репозитории. Время жизни — на запрос (scoped), как у контекста.
    /// </summary>
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<FriendWishDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWishRepository, WishRepository>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();

        return services;
    }
}
