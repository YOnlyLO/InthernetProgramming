using FriendWish.Core.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace FriendWish.Api.Errors;

/// <summary>
/// Превращает исключения домена в ответ ProblemDetails со статусом по типу исключения.
/// Остальные исключения не обрабатывает — их отдаёт как 500 стандартный <c>UseExceptionHandler</c>.
/// </summary>
internal sealed class DomainExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
            return false;

        // Наследники проверяются раньше базового DomainException, иначе любая ошибка стала бы 400.
        // Порядок проверяет компилятор: ветка базового типа выше наследника не скомпилируется.
        var status = domainException switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ForbiddenException => StatusCodes.Status403Forbidden,
            ConflictException => StatusCodes.Status409Conflict,
            InvalidCredentialsException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status400BadRequest
        };

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = status,
                Detail = domainException.Message
            }
        });
    }
}
