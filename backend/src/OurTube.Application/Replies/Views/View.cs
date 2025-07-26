using OurTube.Application.Replies.Video;

namespace OurTube.Application.Replies.Views
{
    /// <summary>
    /// Модель просмотра видео с информацией о самом видео и времени просмотра.
    /// </summary>
    public class View
    {
        /// <summary>
        /// Минимальная информация о видео.
        /// </summary>
        public MinVideo Video { get; set; }

        /// <summary>
        /// Дата и время просмотра видео (в формате UTC).
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}