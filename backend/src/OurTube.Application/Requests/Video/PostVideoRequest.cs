using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OurTube.Application.Requests.Video;

/// <summary>
///     Модель запроса для загрузки нового видео.
/// </summary>
public class PostVideoRequest
{
    /// <summary>
    ///     Название видео (до 150 символов).
    /// </summary>
    [MaxLength(150, ErrorMessage = "Название видео не должно превышать 150 символов")]
    [Required]
    public required string Title { get; set; }

    /// <summary>
    ///     Подробное описание видео (до 5000 символов).
    /// </summary>
    [MaxLength(5000, ErrorMessage = "Описание не должно превышать 5000 символов")]
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Список тегов, связанных с видео.
    ///     Формат: "#{tag}"
    /// </summary>
    public IEnumerable<string> Tags { get; set; } = new List<string>();

    /// <summary>
    ///     Видео-файл.
    /// </summary>
    [Required]
    public required IFormFile VideoFile { get; set; }

    /// <summary>
    ///     Превью-изображение (thumbnail) для видео.
    /// </summary>
    [Required]
    public required IFormFile PreviewFile { get; set; }
}