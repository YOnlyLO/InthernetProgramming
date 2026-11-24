namespace FriendWish.Api.Dtos.Auth;

/// <summary>
/// JWT для заголовка <c>Authorization: Bearer &lt;token&gt;</c>. Срок действия и роль — в самом токене
/// (claims <c>exp</c> и <c>role</c>).
/// </summary>
public sealed record LoginResponse(string AccessToken);
