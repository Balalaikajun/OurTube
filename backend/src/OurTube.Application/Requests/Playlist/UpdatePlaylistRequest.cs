using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.Requests.Playlist;

/// <summary>
///     Запрос на обновление плейлиста.
/// </summary>
public class UpdatePlaylistRequest
{
    /// <summary>
    ///     Название плейлиста.
    ///     Необязательное поле, максимум 150 символов.
    /// </summary>
    [Required(ErrorMessage = "Название плейлиста обязательно.")]
    [MaxLength(150, ErrorMessage = "Название плейлиста не должно превышать 150 символов.")]
    public string? Title { get; set; }
}