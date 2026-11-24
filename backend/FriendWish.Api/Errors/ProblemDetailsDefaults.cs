namespace FriendWish.Api.Errors;

/// <summary>
/// Единый формат ошибок API — ProblemDetails (RFC 9457). Применяется ко всем ошибкам: исключениям домена,
/// ошибкам валидации запроса, 401/403 от авторизации, неизвестным адресам и необработанным исключениям.
/// </summary>
/// <remarks>
/// <c>detail</c> всегда содержит текст для пользователя на русском, поэтому клиенту достаточно показать его.
/// У ошибок валидации запроса дополнительно есть <c>errors</c> — сообщения по полям формы.
/// </remarks>
internal static class ProblemDetailsDefaults
{
    public static void Apply(ProblemDetailsContext context)
    {
        var problem = context.ProblemDetails;
        var status = problem.Status ?? context.HttpContext.Response.StatusCode;

        problem.Title = status switch
        {
            StatusCodes.Status400BadRequest => "Некорректный запрос",
            StatusCodes.Status401Unauthorized => "Не авторизован",
            StatusCodes.Status403Forbidden => "Доступ запрещён",
            StatusCodes.Status404NotFound => "Не найдено",
            StatusCodes.Status409Conflict => "Конфликт",
            StatusCodes.Status500InternalServerError => "Внутренняя ошибка сервера",
            _ => problem.Title
        };

        problem.Detail ??= problem is HttpValidationProblemDetails { Errors.Count: > 0 } validation
            ? string.Join(" ", validation.Errors.SelectMany(error => error.Value).Distinct())
            : status switch
            {
                StatusCodes.Status401Unauthorized => "Войдите в систему.",
                StatusCodes.Status403Forbidden => "Недостаточно прав для этого действия.",
                StatusCodes.Status404NotFound => "Адрес не найден.",
                StatusCodes.Status500InternalServerError => "Произошла непредвиденная ошибка. Попробуйте позже.",
                _ => null
            };
    }
}
