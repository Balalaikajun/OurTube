using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using OurTube.Application.Interfaces;

namespace OurTube.Infrastructure.Other;

public class MinioClient : IStorageClient
{
    private readonly string _accessKey;
    private readonly string _bucketName;
    private readonly string _endpoint; // Обязательно так
    private readonly IMinioClient _minioClient;
    private readonly string _secretKey;

    public MinioClient(IConfiguration configuration)
    {
        _accessKey = configuration["MinIO:AccessKey"];
        _secretKey = configuration["MinIO:SecretKey"];
        _endpoint = configuration["MinIO:Endpoint"];
        _bucketName = configuration["MinIO:VideoBucket"];

        _minioClient = new Minio.MinioClient()
            .WithEndpoint(_endpoint)
            .WithCredentials(_accessKey, _secretKey)
            .WithSSL(false)
            .Build();
    }

    public async Task UploadFilesAsync(string[] inputFiles, string bucket, string prefix)
    {
        Task[] tasks = inputFiles.Select(async f =>
        {
            await UploadFileAsync(
                f,
                Path.Combine(prefix, Path.GetFileName(f)).Replace(@"\", @"/"),
                bucket);
        }).ToArray();
        await Task.WhenAll(tasks);
    }

    public async Task UploadFileAsync(string input, string objectName, string bucketName)
    {
        if (string.IsNullOrEmpty(bucketName))
            throw new ArgumentException("Имя бакета не задано", nameof(bucketName));
        if (string.IsNullOrEmpty(objectName))
            throw new ArgumentException("Имя объекта не задано", nameof(objectName));

        if (!File.Exists(input))
            throw new FileNotFoundException("Файл не найден", input);


        var fileInfo = new FileInfo(input);
        await using var fileStream = fileInfo.OpenRead();

        var args = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileInfo.Length);

        await _minioClient.PutObjectAsync(args);
    }

    public async Task UploadFileAsync(IFormFile input, string objectName, string bucketName)
    {
        if (string.IsNullOrEmpty(bucketName))
            throw new ArgumentException("Имя бакета не задано", nameof(bucketName));
        if (string.IsNullOrEmpty(objectName))
            throw new ArgumentException("Имя объекта не задано", nameof(objectName));

        if (input.Length == 0)
            throw new ArgumentException("Файл не задан или пустой.", nameof(input));

        await using var fileStream = input.OpenReadStream();

        var args = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(input.Length)
            .WithContentType(input.ContentType);

        await _minioClient.PutObjectAsync(args);
    }

    public async Task DeleteFileAsync(string bucketName, string objectName)
    {
        if (string.IsNullOrEmpty(bucketName))
            throw new ArgumentException("Имя бакета не задано", nameof(bucketName));
        if (string.IsNullOrEmpty(objectName))
            throw new ArgumentException("Имя объекта не задано", nameof(objectName));

        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName));
    }
}