using Microsoft.IdentityModel.JsonWebTokens;

namespace FriendWish.Api.Auth;

/// <summary>
/// Claims, которые выпускает <see cref="JwtProvider"/>. Входящие claims не переименовываются
/// (<c>MapInboundClaims = false</c>), поэтому при проверке токена используются те же короткие имена.
/// </summary>
internal static class JwtClaimTypes
{
    public const string UserId = JwtRegisteredClaimNames.Sub;

    public const string UserName = JwtRegisteredClaimNames.UniqueName;

    public const string Role = "role";
}
