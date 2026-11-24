using FriendWish.Api.Extensions;
using FriendWish.DataAccess;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("FriendWish")
    ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:FriendWish.");

builder.Services.AddDataAccess(connectionString);
builder.Services.AddCoreServices();
builder.Services.AddAuthServices(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
