using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PracticalWork.Library.Contracts.Helpers;

/// <summary>
/// Вспомогательный класс для формирования ключей.
/// </summary>
public static class CacheKeyHasher
{
    public static string GenerateCacheKey(string prefix, int version, object parameters)
    {
        var json = JsonSerializer.Serialize(parameters);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json)))
            .Substring(0, 16)
            .ToLowerInvariant();

        return $"{prefix}:v{version}:{hash}";
    }
}
