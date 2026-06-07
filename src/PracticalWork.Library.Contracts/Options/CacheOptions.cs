namespace PracticalWork.Library.Contracts.Options;

/// <summary>
/// Базовые настройки для кеша.
/// </summary>
public class CacheOptions
{
    public string Prefix { get; set; }

    public int TtlMinutes { get; set; }
}
