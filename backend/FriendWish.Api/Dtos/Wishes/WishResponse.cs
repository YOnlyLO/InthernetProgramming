namespace FriendWish.Api.Dtos.Wishes;

public sealed record WishResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string? Description,
    string? Link,
    decimal? Price,
    string? ImageUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt);
