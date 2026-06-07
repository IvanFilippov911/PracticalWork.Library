namespace PracticalWork.Library.Contracts.Abstractions.Services;

/// <summary>
/// Сервис взаимодействия с Minio.
/// </summary>
public interface IMinioService
{
    Task UploadFileAsync(
        string bucket,
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<string> GetFileUrlAsync(string bucket, string fileName, CancellationToken cancellationToken = default);

    Task RemoveFileAsync(string bucket, string fileName, CancellationToken cancellationToken = default);
}
