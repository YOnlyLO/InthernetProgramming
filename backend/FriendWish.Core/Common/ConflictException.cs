namespace FriendWish.Core.Common;

public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message)
    {
    }
}
