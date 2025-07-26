namespace OurTube.Application.Replies.VideoPreview;

/// <summary>
/// Модель превью видео с информацией о файле и его расположении.
/// </summary>
public class VideoPreview
{
    /// <summary>
    /// Имя файла превью изображения.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Имя хранилища (bucket), в котором хранится файл превью.
    /// </summary>
    public required string Bucket { get; set; }
}