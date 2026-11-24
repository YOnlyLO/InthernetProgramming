using FriendWish.Api.Auth;
using FriendWish.Api.Dtos.Profile;
using FriendWish.Api.Mappers;
using FriendWish.Core.Common;
using FriendWish.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendWish.Api.Controllers;

/// <summary>
/// Профиль текущего пользователя.
/// </summary>
[ApiController]
[Route("api/profile")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProfileController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Возвращает свой профиль.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ProfileResponse> Get(CancellationToken cancellationToken)
    {
        // Токен может пережить пользователя, например после пересоздания базы.
        var user = await userService.GetByIdAsync(User.GetUserId(), cancellationToken)
            ?? throw new NotFoundException("Пользователь не найден.");

        return user.ToProfileResponse();
    }

    /// <summary>
    /// Меняет имя и аватар.
    /// </summary>
    [HttpPut]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ProfileResponse> Update(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await userService.UpdateProfileAsync(
            User.GetUserId(),
            request.Name,
            request.AvatarUrl,
            cancellationToken);

        return user.ToProfileResponse();
    }

    /// <summary>
    /// Меняет пароль. Неверный текущий пароль — 400, а не 401, чтобы клиент не разлогинил пользователя.
    /// </summary>
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await userService.ChangePasswordAsync(
            User.GetUserId(),
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        return NoContent();
    }
}
