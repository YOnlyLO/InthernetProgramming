using System.ComponentModel.DataAnnotations;
using FriendWish.Core.Enums;

namespace FriendWish.Api.Dtos.Admin;

public sealed record ChangeRoleRequest
{
    // Nullable, чтобы отсутствие поля было ошибкой, а не тихим значением по умолчанию User.
    /// <summary>Новая роль: <c>User</c> или <c>Admin</c>.</summary>
    [Required(ErrorMessage = "Поле «Роль» обязательно для заполнения.")]
    public UserRole? Role { get; init; }
}
