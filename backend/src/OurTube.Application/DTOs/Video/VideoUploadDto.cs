using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OurTube.Application.DTOs.Video;

/// <summary>
/// Модель запроса для загрузки нового видео.
/// </summary>
public class VideoUploadDto
{
    /// <summary>
    /// Метаданные видео: заголовок, описание, теги.
    /// </summary>
    [Required]
    public VideoPostDto VideoPostDto { get; set; }

    /// <summary>
    /// Видео-файл.
    /// </summary>
    [Required]
    public IFormFile VideoFile { get; set; }

    /// <summary>
    /// Превью-изображение (thumbnail) для видео.
    /// </summary>
    [Required]
    public IFormFile PreviewFile { get; set; }
}