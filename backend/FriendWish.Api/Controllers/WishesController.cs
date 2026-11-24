using FriendWish.Api.Auth;
using FriendWish.Api.Dtos.Wishes;
using FriendWish.Api.Mappers;
using FriendWish.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendWish.Api.Controllers;

/// <summary>
/// Свой список желаний. Список друга — <c>GET /api/users/{id}/wishes</c>.
/// </summary>
[ApiController]
[Route("api/wishes")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class WishesController(IWishService wishService) : ControllerBase
{
    /// <summary>
    /// Возвращает свои желания.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<WishResponse>>(StatusCodes.Status200OK)]
    public async Task<IReadOnlyList<WishResponse>> GetAll(CancellationToken cancellationToken)
    {
        var wishes = await wishService.GetByUserIdAsync(User.GetUserId(), cancellationToken);

        return wishes.Select(wish => wish.ToResponse()).ToList();
    }

    /// <summary>
    /// Добавляет желание в свой список.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<WishResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WishResponse>> Create(WishRequest request, CancellationToken cancellationToken)
    {
        var wish = await wishService.CreateAsync(
            User.GetUserId(),
            request.Title,
            request.Description,
            request.Link,
            request.Price,
            request.ImageUrl,
            cancellationToken);

        // Без Location: отдельного GET /api/wishes/{id} в API нет.
        return StatusCode(StatusCodes.Status201Created, wish.ToResponse());
    }

    /// <summary>
    /// Изменяет своё желание. Чужое желание — 403.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<WishResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<WishResponse> Update(Guid id, WishRequest request, CancellationToken cancellationToken)
    {
        var wish = await wishService.UpdateAsync(
            User.GetUserId(),
            id,
            request.Title,
            request.Description,
            request.Link,
            request.Price,
            request.ImageUrl,
            cancellationToken);

        return wish.ToResponse();
    }

    /// <summary>
    /// Удаляет своё желание. Чужое желание — 403.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await wishService.DeleteAsync(User.GetUserId(), id, cancellationToken);

        return NoContent();
    }
}
