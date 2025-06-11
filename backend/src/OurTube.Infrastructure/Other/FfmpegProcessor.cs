using OurTube.Application.Interfaces;
using Xabe.FFmpeg;

namespace OurTube.Infrastructure.Other;

public class FfmpegProcessor : IVideoProcessor
{
    public async Task HandleVideo(string inputVideo, string outputDir, int videoHeight, string segmentsUri)
    {
        if (!File.Exists(inputVideo))
            throw new FileNotFoundException();

        var inputVideoMediaInfo = await FFmpeg.GetMediaInfo(inputVideo);
        var localSegmentsPath =
            Path.Combine(outputDir, "segments", "segment_%03d.ts"); // Путь до сегментов в локальной папке
        var localPlaylistPath = Path.Combine(outputDir, "playlist.m3u8");
        var segmentsFullUri = Path.Combine(segmentsUri, "segments/").Replace(@"\", @"/");

        Directory.CreateDirectory(Path.Combine(outputDir, "segments"));

        await FFmpeg.Conversions.New()
            .AddStream(inputVideoMediaInfo.Streams)
            .AddParameter($"-vf scale=-1:{videoHeight}") // Масштабируем видео
            .AddParameter("-c:v h264_nvenc -preset p4") // Кодируем видео с x264
            .AddParameter("-cq 18") // Устанавливаем качество
            .AddParameter("-c:a aac -b:a 128k") // Кодируем аудио
            .AddParameter("-hls_time 10 -hls_list_size 0") // HLS настройким
            .AddParameter($"-hls_segment_filename \"{localSegmentsPath}\"")
            .AddParameter($"-hls_base_url \"{segmentsFullUri}\"")
            .SetOutput(localPlaylistPath)
            .Start();
    }

    public async Task<TimeSpan> GetVideoDuration(string filePath)
    {
        var mediaInfo = await FFmpeg.GetMediaInfo(filePath);
        return mediaInfo.Duration;
    }
}