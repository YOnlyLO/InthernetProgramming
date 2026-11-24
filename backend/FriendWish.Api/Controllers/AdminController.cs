using FriendWish.Api.Auth;
using FriendWish.Api.Dtos.Admin;
using FriendWish.Api.Mappers;
using FriendWish.Core.Enums;
using FriendWish.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendWish.Api.Controllers;

/// <summary>
/// Административное управление пользователями. Только для роли Admin, остальным — 403.
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = nameof(UserRole.Admin))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class AdminController(IAdminService adminService) : ControllerBase
{
    /// <summary>
    /// Возвращает всех пользователей в порядке регистрации.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AdminUserResponse>>(StatusCodes.Status200OK)]
    public async Task<IReadOnlyList<AdminUserResponse>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await adminService.GetUsersAsync(cancellationToken);

        return users.Select(user => user.ToAdminUserResponse()).ToList();
    }

    /// <summary>
    /// Меняет роль пользователя. Свою роль изменить нельзя.
    /// </summary>
    [HttpPut("{id:guid}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeRole(Guid id, ChangeRoleRequest request, CancellationToken cancellationToken)
    {
        // Role не null: [Required] проверен до вызова действия.
        await adminService.ChangeRoleAsync(User.GetUserId(), id, request.Role!.Value, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Блокирует пользователя: он больше не сможет войти. Себя заблокировать нельзя.
    /// </summary>
    [HttpPut("{id:guid}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Block(Guid id, CancellationToken cancellationToken)
    {
        await adminService.BlockAsync(User.GetUserId(), id, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Разблокирует пользователя.
    /// </summary>
    [HttpPut("{id:guid}/unblock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unblock(Guid id, CancellationToken cancellationToken)
    {
        await adminService.UnblockAsync(User.GetUserId(), id, cancellationToken);

        return NoContent();
    }
}
