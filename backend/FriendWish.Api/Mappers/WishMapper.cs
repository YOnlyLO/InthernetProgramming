using FriendWish.Api.Dtos.Wishes;
using FriendWish.Core.Models;

namespace FriendWish.Api.Mappers;

internal static class WishMapper
{
    public static WishResponse ToResponse(this Wish wish)
    {
        return new WishResponse(
            wish.Id,
            wish.UserId,
            wish.Title,
            wish.Description,
            wish.Link,
            wish.Price,
            wish.ImageUrl,
            wish.CreatedAt,
            wish.UpdatedAt);
    }
}
