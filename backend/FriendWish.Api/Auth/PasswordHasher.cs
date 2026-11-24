using System.Security.Cryptography;
using FriendWish.Core.Interfaces.Auth;

namespace FriendWish.Api.Auth;

/// <summary>
/// Хэширует пароли PBKDF2 (HMAC-SHA256) со случайной солью. Алгоритм выбран вместо BCrypt:
/// BCrypt учитывает только первые 72 байта пароля, а пароль может быть до 128 символов.
/// </summary>
/// <remarks>
/// Хэш хранится строкой <c>PBKDF2-SHA256$итерации$соль$хэш</c> (соль и хэш — Base64). Число итераций записано
/// в самом хэше, поэтому его можно увеличить позже: старые хэши продолжат проверяться со своим значением.
/// </remarks>
internal sealed class PasswordHasher : IPasswordHasher
{
    private const string Algorithm = "PBKDF2-SHA256";
    private const char Separator = '$';

    // Рекомендация OWASP для PBKDF2-HMAC-SHA256.
    private const int Iterations = 600_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, HashSize);

        return string.Join(
            Separator,
            Algorithm,
            Iterations.ToString(),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool Verify(string password, string passwordHash)
    {
        var parts = passwordHash.Split(Separator);
        if (parts.Length != 4 || parts[0] != Algorithm || !int.TryParse(parts[1], out var iterations) || iterations <= 0)
            return false;

        byte[] salt;
        byte[] expectedHash;
        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedHash = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithm, expectedHash.Length);

        // Сравнение за постоянное время: по времени ответа нельзя подобрать хэш побайтно.
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
