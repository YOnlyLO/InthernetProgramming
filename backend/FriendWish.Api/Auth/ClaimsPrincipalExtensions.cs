using System.Security.Claims;

namespace FriendWish.Api.Auth;

internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Id текущего пользователя из JWT. Контроллеры берут его только отсюда, а не из адреса или тела запроса:
    /// подменой id нельзя получить или изменить чужие данные.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtClaimTypes.UserId);

        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new InvalidOperationException("В токене нет идентификатора пользователя.");
    }
}
