namespace OurTube.Application.Replies.Common;

/// <summary>
///     Модель ошибки, возвращаемая в ответах API.
/// </summary>
public class Error
{
    public Error(string code, int statusCode, string message)
    {
        Code = code;
        StatusCode = statusCode;
        Message = message;
    }

    /// <summary>
    ///     Машинно-обрабатываемый код ошибки (например, <c>UserNotFound</c>, <c>InvalidEmail</c>).
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    ///     HTTP-статус код, связанный с ошибкой (например, <c>400</c>, <c>404</c>, <c>500</c>).
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    ///     Человекочитаемое сообщение об ошибке, предназначенное для отображения пользователю.
    /// </summary>
    public string Message { get; set; }
}