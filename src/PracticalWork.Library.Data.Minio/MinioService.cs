using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Options;

namespace PracticalWork.Library.Data.Minio;

 /// <inheritdoc />
public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;
    private readonly IMinioClient _presignClient;
    private readonly MinioOptions _minioOptions;

    public MinioService(IOptionsMonitor<MinioOptions> minioOptions)
    {
        _minioOptions = minioOptions.CurrentValue;
        _minioClient = new MinioClient()
            .WithEndpoint(_minioOptions.Endpoint)
            .WithCredentials(_minioOptions.AccessKey, _minioOptions.SecretKey)
            .WithSSL(false)
            .Build();
        _presignClient = BuildPresignClient(_minioOptions);
    }

    public async Task UploadFileAsync(string bucket, string fileName, Stream fileStream, string contentType,
        CancellationToken cancellationToken = default)
    {
        var bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucket), cancellationToken);
        if (!bucketExists)
        {
            await _minioClient
                .MakeBucketAsync(new MakeBucketArgs()
                    .WithBucket(bucket), cancellationToken);
        }

        fileStream.Position = 0;
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType), cancellationToken);
    }

    public async Task<string> GetFileUrlAsync(string bucket, string fileName, CancellationToken cancellationToken = default)
    {
        await CheckExistingAsync(bucket, fileName);
        var expiryInSeconds = _minioOptions.ExpInSec;

        var args = new PresignedGetObjectArgs()
            .WithBucket(bucket)
            .WithObject(fileName)
            .WithExpiry(expiryInSeconds);

        return await _presignClient.PresignedGetObjectAsync(args);
    }

    public async Task RemoveFileAsync(string bucket, string fileName, CancellationToken cancellationToken = default)
    {
        await CheckExistingAsync(bucket, fileName);
        await _minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(bucket)
                .WithObject(fileName),
            cancellationToken);
    }
    
    private async Task CheckExistingAsync(string bucket, string fileName)
    {
        try
        {
            var obj = await _minioClient.StatObjectAsync(new StatObjectArgs()
                .WithBucket(bucket)
                .WithObject(fileName));
        }   
        catch (ObjectNotFoundException)
        {
            throw new FileNotFoundException($"Файл {fileName} не существует");
        }
        catch (BucketNotFoundException)
        {
            throw new FileNotFoundException($"Бакет {bucket} не существует");
        }
    }
    
    private static IMinioClient BuildPresignClient(MinioOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.PublicEndpoint))
        {
            return new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(false)
                .Build();
        }

        if (Uri.TryCreate(options.PublicEndpoint, UriKind.Absolute, out var publicUri))
        {
            var hostPort = publicUri.IsDefaultPort
                ? publicUri.Host
                : $"{publicUri.Host}:{publicUri.Port}";

            return new MinioClient()
                .WithEndpoint(hostPort)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(string.Equals(publicUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
                .Build();
        }

        return new MinioClient()
            .WithEndpoint(options.PublicEndpoint)
            .WithCredentials(options.AccessKey, options.SecretKey)
            .WithSSL(false)
            .Build();
    }
}
