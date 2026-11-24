using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FriendWish.Api.Auth;

/// <summary>
/// Настройки JWT из секции <c>Jwt</c>. Проверяются при старте приложения: без ключа подписи оно не запустится.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "Не задан издатель токена Jwt:Issuer.")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Не задан получатель токена Jwt:Audience.")]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Ключ подписи HMAC-SHA256. Алгоритм требует ключ не короче 256 бит, то есть 32 байт.
    /// </summary>
    [Required(ErrorMessage = "Не задан ключ подписи токена Jwt:SecretKey.")]
    [MinLength(32, ErrorMessage = "Ключ подписи токена Jwt:SecretKey должен быть не короче 32 символов.")]
    public string SecretKey { get; set; } = string.Empty;

    [Range(1, 24 * 60, ErrorMessage = "Срок действия токена Jwt:ExpiresMinutes — от 1 до 1440 минут.")]
    public int ExpiresMinutes { get; set; } = 60;

    public SymmetricSecurityKey CreateSigningKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
    }
}
