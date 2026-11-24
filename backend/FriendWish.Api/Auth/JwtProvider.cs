using FriendWish.Core.Interfaces.Auth;
using FriendWish.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FriendWish.Api.Auth;

/// <summary>
/// Выпускает JWT, подписанный HMAC-SHA256. В токене — id пользователя, юзернейм и роль:
/// по роли работает <c>[Authorize(Roles = "Admin")]</c>, а frontend решает, показывать ли админ-панель.
/// </summary>
internal sealed class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JsonWebTokenHandler _tokenHandler = new();

    public string GenerateToken(User user)
    {
        var jwt = options.Value;
        var now = DateTime.UtcNow;

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddMinutes(jwt.ExpiresMinutes),
            Claims = new Dictionary<string, object>
            {
                [JwtClaimTypes.UserId] = user.Id.ToString(),
                [JwtClaimTypes.UserName] = user.UserName,
                [JwtClaimTypes.Role] = user.Role.ToString()
            },
            SigningCredentials = new SigningCredentials(jwt.CreateSigningKey(), SecurityAlgorithms.HmacSha256)
        };

        return _tokenHandler.CreateToken(descriptor);
    }
}
