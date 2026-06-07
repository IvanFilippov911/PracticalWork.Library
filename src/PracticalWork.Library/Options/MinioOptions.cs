namespace PracticalWork.Library.Options;

/// <summary>
/// Настройки подключения и конфигурации для Minio
/// </summary>
public class MinioOptions
{
    /// <summary>
    /// URL-адрес сервера Minio
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// Публичный URL Minio для presigned ссылок (например http://localhost:9003)
    /// </summary>
    public string PublicEndpoint { get; set; }

    /// <summary>
    /// Ключ доступа к Minio
    /// </summary>
    public string AccessKey { get; set; }

    /// <summary>
    /// Секретный ключ доступа к Minio
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    /// Название бакета для отчетов
    /// </summary>
    public string ReportsBucketName { get; set; }

    /// <summary>
    /// Название бакета для обложек книг
    /// </summary>
    public string CoversBucketName { get; set; }

    /// <summary>
    /// Время жизни ссылки в секундах
    /// </summary>
    public int ExpInSec { get; set; }
}
