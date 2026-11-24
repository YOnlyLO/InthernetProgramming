using FriendWish.Api.Auth;
using FriendWish.Api.Dtos.Users;
using FriendWish.Api.Dtos.Wishes;
using FriendWish.Api.Mappers;
using FriendWish.Core.Common;
using FriendWish.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendWish.Api.Controllers;

/// <summary>
/// Поиск пользователей, публичный профиль и список желаний друга.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UsersController(IUserService userService, IWishService wishService) : ControllerBase
{
    /// <summary>
    /// Ищет пользователей по подстроке имени или юзернейма без учёта регистра — не больше 20.
    /// Себя и заблокированных в результате нет; пустой запрос — пустой список.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType<IReadOnlyList<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<IReadOnlyList<UserResponse>> Search(
        [FromQuery] string? query,
        CancellationToken cancellationToken)
    {
        var users = await userService.SearchAsync(User.GetUserId(), query ?? string.Empty, cancellationToken);

        return users.Select(user => user.ToUserResponse()).ToList();
    }

    /// <summary>
    /// Возвращает публичный профиль пользователя.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<UserResponse> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Пользователь не найден.");

        return user.ToUserResponse();
    }

    /// <summary>
    /// Возвращает список желаний друга. Если пользователь не в друзьях — 403.
    /// </summary>
    [HttpGet("{id:guid}/wishes")]
    [ProducesResponseType<IReadOnlyList<WishResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IReadOnlyList<WishResponse>> GetWishes(Guid id, CancellationToken cancellationToken)
    {
        var wishes = await wishService.GetFriendWishesAsync(User.GetUserId(), id, cancellationToken);

        return wishes.Select(wish => wish.ToResponse()).ToList();
    }
}
