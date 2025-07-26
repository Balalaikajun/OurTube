using OurTube.Application.Requests.Video;

namespace OurTube.Application.Validators;

public class VideoValidator
{
    public void ValidateVideo(PostVideoRequest video)
    {
        // Валидация данных
        if (!video.VideoFile.ContentType.StartsWith("video/"))
            throw new FormatException($"Формат файла: {video.VideoFile.ContentType} - не поддерживается");

        if (!video.PreviewFile.ContentType.StartsWith("image/"))
            throw new FormatException($"Формат файла: {video.PreviewFile.ContentType} - не поддерживается");

        if (video.Title == "con")
            throw new FormatException("Данное название не соответствует политике компании");
    }
}