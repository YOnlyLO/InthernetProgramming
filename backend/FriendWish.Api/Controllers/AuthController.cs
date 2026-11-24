using FriendWish.Api.Dtos.Auth;
using FriendWish.Api.Dtos.Profile;
using FriendWish.Api.Mappers;
using FriendWish.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendWish.Api.Controllers;

/// <summary>
/// Регистрация и вход. Доступны без токена.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Регистрирует пользователя с ролью User. Токен не выдаёт — после регистрации нужно войти.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProfileResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var user = await authService.RegisterAsync(
            request.Name,
            request.UserName,
            request.Email,
            request.Password,
            cancellationToken);

        return CreatedAtAction(
            nameof(UsersController.GetById),
            "Users",
            new { id = user.Id },
            user.ToProfileResponse());
    }

    /// <summary>
    /// Проверяет email и пароль и выдаёт JWT. Заблокированный пользователь получает 403.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<LoginResponse> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var token = await authService.LoginAsync(request.Email, request.Password, cancellationToken);

        return new LoginResponse(token);
    }
}
