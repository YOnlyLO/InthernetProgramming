using FriendWish.Api.Dtos.Admin;
using FriendWish.Api.Dtos.Profile;
using FriendWish.Api.Dtos.Users;
using FriendWish.Core.Models;

namespace FriendWish.Api.Mappers;

/// <summary>
/// Один пользователь — три представления: каждому потребителю ровно те поля, которые ему можно видеть.
/// </summary>
internal static class UserMapper
{
    public static ProfileResponse ToProfileResponse(this User user)
    {
        return new ProfileResponse(
            user.Id,
            user.Name,
            user.UserName,
            user.Email,
            user.AvatarUrl,
            user.Role,
            user.CreatedAt);
    }

    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse(user.Id, user.Name, user.UserName, user.AvatarUrl);
    }

    public static AdminUserResponse ToAdminUserResponse(this User user)
    {
        return new AdminUserResponse(
            user.Id,
            user.Name,
            user.UserName,
            user.Email,
            user.Role,
            user.Status,
            user.CreatedAt);
    }
}
