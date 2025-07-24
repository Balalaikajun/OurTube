using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.DTOs.Video;

/// <summary>
/// Модель метаданных для создаваемого видео.
/// </summary>
public class VideoPostDto
{
    /// <summary>
    /// Название видео (до 150 символов).
    /// </summary>
    [MaxLength(150, ErrorMessage = "Название видео не должно превышать 150 символов")]
    [Required]
    public string Title { get; set; }

    /// <summary>
    /// Подробное описание видео (до 5000 символов).
    /// </summary>
    [MaxLength(5000, ErrorMessage = "Описание не должно превышать 5000 символов")]
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Список тегов, связанных с видео.
    /// Формат: "#{tag}"
    /// </summary>
    public IEnumerable<string> Tags { get; set; } = new List<string>();
}