using FriendWish.Core.Models;

namespace FriendWish.Core.Interfaces.Auth;

public interface IJwtProvider
{
    string GenerateToken(User user);
}
