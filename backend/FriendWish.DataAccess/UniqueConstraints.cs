namespace FriendWish.DataAccess;

/// <summary>
/// Имена уникальных индексов и сообщения для пользователя, когда запись нарушает один из них.
/// </summary>
internal static class UniqueConstraints
{
    public const string UserEmail = "ux_users_email";
    public const string UserName = "ux_users_user_name";

    /// <summary>
    /// Пара пользователей без учёта направления заявки. Индекс по выражению создаёт миграция InitialCreate:
    /// конфигурация EF Core такой индекс описать не может.
    /// </summary>
    public const string FriendshipPair = "ux_friendships_pair";

    public static string GetConflictMessage(string? constraintName)
    {
        return constraintName switch
        {
            UserEmail => "Пользователь с таким email уже зарегистрирован.",
            UserName => "Этот юзернейм уже занят.",
            FriendshipPair => "Заявка между этими пользователями уже существует.",
            _ => "Запись с такими данными уже существует."
        };
    }
}
