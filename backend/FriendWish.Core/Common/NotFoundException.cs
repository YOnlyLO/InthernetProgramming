namespace FriendWish.Core.Common;

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
