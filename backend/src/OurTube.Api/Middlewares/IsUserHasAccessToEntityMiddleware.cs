using System.Security.Claims;
using System.Text.Json;
using OurTube.Api.Attributes;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Middlewares;

/// <summary>
/// Middleware проверки прав пользователя на доступ к сущности
/// </summary>
public class IsUserHasAccessToEntityMiddleware( RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAccessChecker accessChecker)
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId) || userId == Guid.Empty)
        {
            await WriteErrorAsync(context,
                "Пользователь не авторизован",
                StatusCodes.Status401Unauthorized);
            return;
        }
        
        var attribute = context.GetEndpoint()?
            .Metadata
            .GetMetadata<IsUserHasAccessToEntityAttribute>();
        if (attribute == null)
        {
            await next(context);
            return;
        }

        string? entityStringId = null;
        var idParam = attribute.FromQuery ??
                      attribute.FromRoute ??
                      attribute.FromBody ??
                      attribute.FromForm;

        if (!string.IsNullOrWhiteSpace(attribute.FromQuery))
        {
            context.Request.Query.TryGetValue(attribute.FromQuery, out var vals);
            entityStringId = vals.FirstOrDefault();
        }
        else if (!string.IsNullOrWhiteSpace(attribute.FromRoute))
        {
            context.Request.RouteValues.TryGetValue(attribute.FromRoute, out var rv);
            entityStringId = rv?.ToString();
        }
        else if (!string.IsNullOrWhiteSpace(attribute.FromBody))
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            string body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; 

            using var json = JsonDocument.Parse(body);
            if (json.RootElement.TryGetProperty(attribute.FromBody, out var prop))
                entityStringId = prop.GetString();
        }
        else if (!string.IsNullOrWhiteSpace(attribute.FromForm))
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty(attribute.FromBody, out var prop))
                entityStringId = prop.GetString();
        }
        else
        {
            await context.Response.WriteAsJsonAsync(new
            {
                Code = StatusCodes.Status500InternalServerError,
                Message = "Должен быть задан ровно один из параметров: FromQuery, FromRoute, FromForm или FromBody"
            });

            await context.Response.CompleteAsync();
            return;
        }

        if (string.IsNullOrWhiteSpace(entityStringId) || !Guid.TryParse(entityStringId, out var entityId))
        {
            await WriteErrorAsync(context,
                "Идентификатор сущности в запросе не найден",
                StatusCodes.Status500InternalServerError);
            return;
        }

        if (!await accessChecker.CanEditAsync(attribute.EntityType, entityId, userId))
        {
            await WriteErrorAsync(context,
                "Вы не имеете прав доступа на это действие",
                StatusCodes.Status403Forbidden);
            return;
        }

        await next(context);
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        string message,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear(); // очищаем любые предыдущие данные
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var error = new
        {
            Code = statusCode.ToString(),
            Status = statusCode,
            Message = message,
            TraceId = context.TraceIdentifier
        };

        await context.Response.WriteAsJsonAsync(error);
        await context.Response.CompleteAsync();
    }
}