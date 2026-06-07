namespace PracticalWork.Library.Contracts.Options;

/// <summary>
/// Настройки подключения и конфигурации для Minio.
/// </summary>
public class MinioOptions
{
    public string Endpoint { get; set; }

    public string PublicEndpoint { get; set; }

    public string AccessKey { get; set; }

    public string SecretKey { get; set; }

    public string ReportsBucketName { get; set; }

    public string CoversBucketName { get; set; }

    public int ExpInSec { get; set; }
}
