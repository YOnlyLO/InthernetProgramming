# FriendWish — backend

REST API на ASP.NET Core (.NET 10) с хранением данных в PostgreSQL через EF Core. Аутентификация — JWT, документация — Swagger.

## Структура

| Проект                | Назначение                                                                            |
| --------------------- | ------------------------------------------------------------------------------------- |
| `FriendWish.Core`       | Доменные модели, правила валидации, сервисы с бизнес-логикой, интерфейсы репозиториев |
| `FriendWish.DataAccess` | EF Core: сущности, конфигурации, `DbContext`, репозитории, миграции                   |
| `FriendWish.Api`        | Контроллеры, DTO, JWT и хэширование паролей, обработка ошибок, Swagger                |

## Требования

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL (проверено на версии 18)
- Инструмент миграций EF Core: `dotnet tool install --global dotnet-ef`

## Запуск

Команды выполняются из корня репозитория.

1. **Строка подключения.** По умолчанию в `appsettings.Development.json` указан пользователь `postgres` с паролем `postgres`.
   Если пароль другой, задайте строку подключения через user-secrets — так пароль не попадёт в репозиторий:

   ```bash
   dotnet user-secrets set "ConnectionStrings:FriendWish" "Host=localhost;Port=5432;Database=friendwish;Username=postgres;Password=<пароль>" --project backend/FriendWish.Api
   ```

2. **База данных.** Команда создаёт базу `friendwish`, если её нет, и применяет миграции:

   ```bash
   dotnet ef database update --project backend/FriendWish.DataAccess --startup-project backend/FriendWish.Api
   ```

3. **Запуск API:**

   ```bash
   dotnet run --project backend/FriendWish.Api
   ```

   API доступно на http://localhost:5137, Swagger — на http://localhost:5137/swagger.

## Работа через Swagger

1. Зарегистрируйтесь — `POST /api/auth/register`.
2. Войдите — `POST /api/auth/login` — и скопируйте `accessToken` из ответа.
3. Нажмите **Authorize** и вставьте токен (без слова `Bearer`). Методы с замком станут доступны.

Токен действует 60 минут. Роль записана в токене, поэтому после смены роли нужно войти заново.

## Первый администратор

Регистрация всегда создаёт пользователя с ролью `User`. Первого администратора назначают в базе данных:

```sql
UPDATE users SET role = 'Admin' WHERE email = 'admin@example.com';
```

Дальше роли меняет администратор через `PUT /api/admin/users/{id}/role`.

## Настройки JWT

Секция `Jwt` в `appsettings.json`: издатель `Issuer`, получатель `Audience`, срок действия `ExpiresMinutes`.
Ключ подписи `Jwt:SecretKey` (не короче 32 символов) для разработки лежит в `appsettings.Development.json`.
В других окружениях его задают переменной окружения `Jwt__SecretKey`; без ключа приложение не запустится.

## Формат ошибок

Все ошибки возвращаются в формате [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457) с типом `application/problem+json`.
В поле `detail` — сообщение для пользователя на русском. У ошибок валидации запроса есть ещё поле `errors` с сообщениями по полям:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Некорректный запрос",
  "status": 400,
  "detail": "Пароли не совпадают.",
  "errors": { "confirmPassword": ["Пароли не совпадают."] }
}
```

| Статус | Когда                                                                     |
| ------ | ------------------------------------------------------------------------- |
| 400    | Данные не прошли проверку                                                 |
| 401    | Нет токена, токен недействителен или неверные email и пароль при входе    |
| 403    | Нет прав: чужое желание, список не-друга, админ-метод, заблокированный вход |
| 404    | Запись не найдена                                                         |
| 409    | Конфликт: email или юзернейм заняты, заявка уже отправлена                |
