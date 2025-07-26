namespace OurTube.Application.Requests.Recommendation;

/// <summary>
/// Запрос на получение рекомендованных видео с поддержкой пагинации.
/// </summary>
public class GetRecommendationsForVideoRequest
{
    /// <summary>
    /// Идентификатор видео, для которого формируются рекомендации. 
    /// </summary>
    public Guid? VideoId { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя, для которого формируются рекомендации. 
    /// Может быть <see langword="null"/>, если пользователь не авторизован.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Идентификатор сессии пользователя. Используется для трекинга рекомендаций.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Максимальное количество элементов в выдаче за один запрос.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Смещение или идентификатор элемента, после которого нужно вернуть следующие результаты.
    /// </summary>
    public int After { get; set; }

    /// <summary>
    /// Признак того, что необходимо принудительно обновить рекомендации (игнорировать кэш).
    /// </summary>
    public bool Reload { get; set; } = false;
}