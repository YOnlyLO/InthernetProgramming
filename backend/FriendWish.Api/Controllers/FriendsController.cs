using FriendWish.Api.Auth;
using FriendWish.Api.Dtos.Friends;
using FriendWish.Api.Dtos.Users;
using FriendWish.Api.Mappers;
using FriendWish.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendWish.Api.Controllers;

/// <summary>
/// Друзья и заявки в друзья текущего пользователя.
/// </summary>
[ApiController]
[Route("api/friends")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class FriendsController(IFriendshipService friendshipService) : ControllerBase
{
    /// <summary>
    /// Возвращает список друзей по имени.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<IReadOnlyList<UserResponse>> GetFriends(CancellationToken cancellationToken)
    {
        var friends = await friendshipService.GetFriendsAsync(User.GetUserId(), cancellationToken);

        return friends.Select(friend => friend.ToUserResponse()).ToList();
    }

    /// <summary>
    /// Возвращает входящие и исходящие заявки, ожидающие ответа.
    /// </summary>
    [HttpGet("requests")]
    [ProducesResponseType<FriendRequestsResponse>(StatusCodes.Status200OK)]
    public async Task<FriendRequestsResponse> GetRequests(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var incoming = await friendshipService.GetIncomingRequestsAsync(userId, cancellationToken);
        var outgoing = await friendshipService.GetOutgoingRequestsAsync(userId, cancellationToken);

        return new FriendRequestsResponse(
            incoming.Select(request => request.ToResponse()).ToList(),
            outgoing.Select(request => request.ToResponse()).ToList());
    }

    /// <summary>
    /// Отправляет заявку пользователю. Отклонённая ранее заявка отправляется повторно,
    /// а встречная заявка сразу делает пользователей друзьями (статус Accepted).
    /// </summary>
    [HttpPost("requests/{userId:guid}")]
    [ProducesResponseType<FriendshipResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<FriendshipResponse> SendRequest(Guid userId, CancellationToken cancellationToken)
    {
        var friendship = await friendshipService.SendRequestAsync(User.GetUserId(), userId, cancellationToken);

        return friendship.ToResponse();
    }

    /// <summary>
    /// Принимает входящую заявку.
    /// </summary>
    [HttpPost("requests/{id:guid}/accept")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptRequest(Guid id, CancellationToken cancellationToken)
    {
        await friendshipService.AcceptRequestAsync(User.GetUserId(), id, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Отклоняет входящую заявку.
    /// </summary>
    [HttpPost("requests/{id:guid}/decline")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeclineRequest(Guid id, CancellationToken cancellationToken)
    {
        await friendshipService.DeclineRequestAsync(User.GetUserId(), id, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Удаляет пользователя из друзей.
    /// </summary>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFriend(Guid userId, CancellationToken cancellationToken)
    {
        await friendshipService.RemoveFriendAsync(User.GetUserId(), userId, cancellationToken);

        return NoContent();
    }
}
