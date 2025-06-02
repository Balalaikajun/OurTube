using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using OurTube.Application.Interfaces;

namespace OurTube.Infrastructure.Other
{
    public class MinioService:IBlobService
    {
        private readonly IMinioClient _minioClient;

        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _endpoint; // Обязательно так
        private readonly string _bucketName;

        public MinioService(IConfiguration configuration)
        {
            _accessKey = configuration["Minio:AccessKey"];
            _secretKey = configuration["Minio:SecretKey"];
            _endpoint = configuration["Minio:Endpoint"];
            _bucketName = configuration["Minio:VideoBucket"];

            _minioClient = new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(_accessKey, _secretKey)
                .WithSSL(false)
                .Build();
        }

        public async Task UploadFiles(string[] inputFiles, string bucket, string prefix)
        {
            //Загрузка сегментов, аж асинхронно
            Task[] tasks = inputFiles.Select(async f =>
            {
                await UploadFile(
                    f,
                    Path.Combine(prefix, Path.GetFileName(f)).Replace(@"\", @"/"),
                    bucket);
            }).ToArray();
            await Task.WhenAll(tasks);
        }

        public async Task UploadFile(string input, string objectName, string bucket)
        {
            if (!File.Exists(input))
                throw new FileNotFoundException("Файл не найден", input);
            
            try
            {
                var fileInfo = new FileInfo(input);
                await using var fileStream = fileInfo.OpenRead();

                var args = new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(objectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileInfo.Length);

                await _minioClient.PutObjectAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MinIO Error: {ex.Message}");
                throw;
            }
        }
    }
}
