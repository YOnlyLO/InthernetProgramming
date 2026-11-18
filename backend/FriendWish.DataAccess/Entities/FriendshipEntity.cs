using FriendWish.Core.Enums;

namespace FriendWish.DataAccess.Entities;

public class FriendshipEntity
{
    public Guid Id { get; set; }

    public Guid RequesterId { get; set; }

    public Guid AddresseeId { get; set; }

    public FriendshipStatus Status { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? RespondedAt { get; set; }
}
