namespace FriendWish.Core.Common;

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() : base("Неверный email или пароль.")
    {
    }
}
