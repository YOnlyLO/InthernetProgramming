using FriendWish.Api.Auth;
using FriendWish.Core.Interfaces.Auth;
using FriendWish.Core.Interfaces.Services;
using FriendWish.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FriendWish.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует доменные сервисы Core. Core не зависит от DI, поэтому регистрация живёт в API.
    /// Время жизни — на запрос (scoped), как у репозиториев и <c>DbContext</c>, от которых сервисы зависят.
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWishService, WishService>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }

    /// <summary>
    /// Регистрирует хэширование паролей, выпуск JWT, JWT-аутентификацию и авторизацию по ролям.
    /// </summary>
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Без состояния и без зависимостей от запроса — хватает одного экземпляра на приложение.
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        // Токен проверяется по тем же JwtOptions, по которым выпущен, причём уже после их валидации.
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                // Иначе sub и role переименуются в длинные ClaimTypes.* из WS-Federation.
                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = jwt.CreateSigningKey(),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    NameClaimType = JwtClaimTypes.UserName,
                    RoleClaimType = JwtClaimTypes.Role,

                    // Токен выпускает и проверяет один сервер: расхождения часов нет, допуск по умолчанию (5 мин) не нужен.
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }
}
