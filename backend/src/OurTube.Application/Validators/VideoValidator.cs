using OurTube.Application.Requests.Video;

namespace OurTube.Application.Validators;

public class VideoValidator
{
    public void ValidateVideo(PostVideoRequest video)
    {
        // Валидация данных
        if (!video.VideoFile.ContentType.StartsWith("video/"))
            throw new FormatException($"Формат файла: {video.VideoFile.ContentType} - не поддерживается для {nameof(video.VideoFile)}");

        if (!video.PreviewFile.ContentType.StartsWith("image/"))
            throw new FormatException($"Формат файла: {video.PreviewFile.ContentType} - не поддерживается для {nameof(video.PreviewFile)}");

        if (video.Title == "con")
            throw new FormatException("Данное название видео не соответствует политике компании");
    }
}