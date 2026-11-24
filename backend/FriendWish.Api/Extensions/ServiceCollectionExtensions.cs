using System.Text.Json.Serialization;
using FriendWish.Api.Auth;
using FriendWish.Api.Errors;
using FriendWish.Api.OpenApi;
using FriendWish.Core.Interfaces.Auth;
using FriendWish.Core.Interfaces.Services;
using FriendWish.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

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

    /// <summary>
    /// Регистрирует контроллеры и формат JSON: enum — строками (<c>"Admin"</c>, а не <c>1</c>).
    /// </summary>
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                // Ключи ошибок валидации — по именам полей JSON (confirmPassword), а не свойств C# (ConfirmPassword).
                options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Генератор OpenAPI строит схемы по этим настройкам, а не по MVC: без них enum в документе был бы числом.
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return services;
    }

    /// <summary>
    /// Регистрирует единый формат ошибок: ProblemDetails для исключений домена, ошибок валидации и статусов без тела.
    /// </summary>
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<DomainExceptionHandler>();
        services.AddProblemDetails(options => options.CustomizeProblemDetails = ProblemDetailsDefaults.Apply);

        return services;
    }

    /// <summary>
    /// Регистрирует документ OpenAPI (<c>/openapi/v1.json</c>) со схемой авторизации Bearer для Swagger UI.
    /// </summary>
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "FriendWish API",
                    Version = "v1",
                    Description = "REST API сервиса списков желаний FriendWish. Чтобы вызвать защищённые методы, "
                        + "войдите через POST /api/auth/login и вставьте accessToken в окно «Authorize»."
                };

                return Task.CompletedTask;
            });
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }
}
