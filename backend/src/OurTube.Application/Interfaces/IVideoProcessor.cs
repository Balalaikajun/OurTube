namespace OurTube.Application.Interfaces;

public interface IVideoProcessor
{
    Task HandleVideo(string inputVideo, string outputDir, int videoHeight, string segmentsUri);
    Task<TimeSpan> GetVideoDuration(string filePath);
}