namespace OurTube.Application.Requests.Video
{
    /// <summary>
    /// Запрос на поиск видео с поддержкой пагинации.
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// Строка поискового запроса.
        /// </summary>
        public string SearchQuery { get; set; }

        /// <summary>
        /// Идентификатор пользователя (если он авторизован).
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Идентификатор сессии пользователя.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// Количество элементов в выдаче за один запрос (по умолчанию 10).
        /// </summary>
        public int Limit { get; set; } = 10;

        /// <summary>
        /// Смещение или идентификатор элемента, после которого нужно вернуть следующие результаты.
        /// </summary>
        public int After { get; set; } = 0;

        /// <summary>
        /// Признак того, что нужно принудительно обновить поиск (игнорировать кэш).
        /// </summary>
        public bool Reload { get; set; } = true;
    }
}