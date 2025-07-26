namespace OurTube.Application.Replies.Common;

/// <summary>
/// Универсальная модель для передачи данных с постраничной навигацией (пагинацией).
/// </summary>
/// <typeparam name="T">Тип элементов в коллекции.</typeparam>
public class ListReply<T> where T : class
{
    /// <summary>
    /// Коллекция элементов текущей страницы.
    /// </summary>
    public required IEnumerable<T> Elements { get; set; }

    /// <summary>
    /// Параметр, по которому можно запросить следующую страницу данных (например, ID или индекс).
    /// </summary>
    public int? NextAfter { get; set; }

    /// <summary>
    /// Указывает, есть ли ещё данные после текущей страницы.
    /// </summary>
    public bool HasMore { get; set; }
}