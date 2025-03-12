using Microsoft.Extensions.Configuration;
using Minio;

namespace OurTube.Infrastructure.Other
{
    public class MinioService
    {
        private readonly IMinioClient _minioClient;

        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _endpoint; // Обязательно так
        private readonly string _bucketName;

        public MinioService() { }
        public MinioService(IConfiguration configuration)
        {
            _accessKey = configuration["Minio:AccesKey"];
            _secretKey = configuration["Minio:SecretKey"];
            _endpoint = configuration["Minio:Endpoint"];
            _bucketName = configuration["Minio:VideoBucket"];

            _minioClient = new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(_accessKey, _secretKey)
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

        public async Task UploadFile(string input, string objejtName, string bucket)
        {
            using (var fileStream = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                await _minioClient.PutObjectAsync(
                    new Minio.DataModel.Args.PutObjectArgs()
                    .WithBucket(bucket)
                    .WithStreamData(fileStream)
                    .WithObject(objejtName)
                    .WithObjectSize(fileStream.Length));
            }
        }
    }
}
