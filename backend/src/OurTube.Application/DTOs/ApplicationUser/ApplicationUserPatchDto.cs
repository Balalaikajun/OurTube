namespace OurTube.Application.DTOs.ApplicationUser;

/// <summary>
/// Запрос на обновление данных пользователя
/// </summary>
public class ApplicationUserPatchDto
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string? UserName { get; set; }
}