using FriendWish.Api.Extensions;
using FriendWish.DataAccess;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("FriendWish")
    ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:FriendWish.");

builder.Services.AddDataAccess(connectionString);
builder.Services.AddCoreServices();
builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddApiControllers();
builder.Services.AddApiErrorHandling();
builder.Services.AddApiDocumentation();

var app = builder.Build();

// Первыми в конвейере: исключения и ответы без тела (401, 403, 404) из любого места превращаются в ProblemDetails.
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "FriendWish API v1");
        options.DocumentTitle = "FriendWish API";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
