namespace PracticalWork.Library.Options
{
    /// <summary>
    /// Базовые настройки для кеша
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// Префикс
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Время жизни в минутах
        /// </summary>
        public int TtlMinutes { get; set; }
    }
}