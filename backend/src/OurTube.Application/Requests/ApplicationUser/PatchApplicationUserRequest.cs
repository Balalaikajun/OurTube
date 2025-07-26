namespace OurTube.Application.Requests.ApplicationUser;

/// <summary>
/// Запрос на обновление данных пользователя
/// </summary>
public class PatchApplicationUserRequest
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string? UserName { get; set; }
}