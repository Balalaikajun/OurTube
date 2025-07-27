using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.Requests.Playlist;

/// <summary>
///     Запрос на создание нового плейлиста.
/// </summary>
public class PostPlaylistRequest
{
    /// <summary>
    ///     Название плейлиста.
    ///     Обязательное поле, максимум 150 символов.
    /// </summary>
    [Required(ErrorMessage = "Название плейлиста обязательно.")]
    [MaxLength(150, ErrorMessage = "Название плейлиста не должно превышать 150 символов.")]
    public required string Title { get; set; }
}