using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using System.Threading;

namespace HostingPrototype.Services
{
    public class MinioService
    {
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
        }

        public async Task UploadSlicedVideo(string localDir, string videoId, int height)
        {
            //Локальное расположение
            string localPlaylistPath = Path.Combine(localDir, height.ToString(), "playlist.m3u8");
            string localSegmentsDir = Path.Combine(localDir, height.ToString(), "segments");

            //Расположение в MinIO
            string minioPlaylistPath = Path.Combine( videoId, height.ToString(), "playlist.m3u8")
                .Replace(Path.DirectorySeparatorChar, '/');
            string minioSegmentsDir = Path.Combine(videoId, height.ToString(), "segments")
                .Replace(Path.DirectorySeparatorChar, '/');


            //Проверка наличия плейлиста
            if (!File.Exists(localPlaylistPath))
                throw new FileNotFoundException("Playlist was not found.");

            //Проверка количества сегментов
            int segmentsCount = File.ReadLines(localPlaylistPath).Where(x => x.StartsWith("#EXTINF")).Count();
            if (Directory.GetFiles(localSegmentsDir).Length != segmentsCount)
                throw new FileNotFoundException($"Segments count in playlist does not match with count files in {localSegmentsDir}");

            using var client = new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(_accessKey, _secretKey)
                .Build();


            //Загрузка сегментов, аж асинхронно
            List<Task> tasks = new List<Task>();
            foreach (string segmentPath in Directory.GetFiles(localSegmentsDir))
            {
                tasks.Add(
                    client.PutObjectAsync(
                        new Minio.DataModel.Args.PutObjectArgs()
                        .WithBucket(_bucketName)
                        .WithObject(minioSegmentsDir+"/"+Path.GetFileName(segmentPath))
                        .WithFileName(segmentPath)));
            }
            await Task.WhenAll(tasks);


            //Загрузка плейлиста
            await client.PutObjectAsync(
                        new Minio.DataModel.Args.PutObjectArgs()
                        .WithBucket(_bucketName)
                        .WithObject(minioPlaylistPath)
                        .WithFileName(localPlaylistPath));


        }
    }
}
