namespace FriendWish.Core.Common;

internal static class Guard
{
    public static Guid NotEmpty(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
            throw new DomainException($"Поле «{fieldName}» должно содержать идентификатор.");

        return value;
    }

    public static string Required(string? value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"Поле «{fieldName}» обязательно для заполнения.");

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new DomainException($"Длина поля «{fieldName}» не должна превышать {maxLength} симв.");

        return trimmed;
    }

    public static string? Optional(string? value, int maxLength, string fieldName)
    {
        return string.IsNullOrWhiteSpace(value) ? null : Required(value, maxLength, fieldName);
    }

    public static string? OptionalUrl(string? value, int maxLength, string fieldName)
    {
        var url = Optional(value, maxLength, fieldName);
        if (url is null)
            return null;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new DomainException($"Поле «{fieldName}» должно содержать корректную ссылку (http или https).");

        return url;
    }

    public static TEnum DefinedEnum<TEnum>(TEnum value, string fieldName) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new DomainException($"Недопустимое значение поля «{fieldName}».");

        return value;
    }
}
