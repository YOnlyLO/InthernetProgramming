namespace FriendWish.Core.Common;

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
