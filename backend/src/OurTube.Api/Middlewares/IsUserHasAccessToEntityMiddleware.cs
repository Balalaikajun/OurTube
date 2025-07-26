using System.Security.Claims;
using System.Text.Json;
using OurTube.Api.Attributes;
using OurTube.Application.DTOs.Common;
using OurTube.Application.Interfaces;
using OurTube.Domain.Exceptions;

namespace OurTube.Api.Middlewares;

/// <summary>
/// Middleware проверки прав пользователя на доступ к сущности
/// </summary>
public class IsUserHasAccessToEntityMiddleware( RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAccessChecker accessChecker)
    {
       var attribute = context.GetEndpoint()?
            .Metadata
            .GetMetadata<IsUserHasAccessToEntityAttribute>();
       
        if (attribute == null)
        {
            await next(context);
            return;
        }
        
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId) || userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException();
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
            throw new InvalidOperationException(
                "Должен быть задан ровно один из параметров: FromQuery, FromRoute, FromForm или FromBody");
        }

        if (string.IsNullOrWhiteSpace(entityStringId) || !Guid.TryParse(entityStringId, out var entityId))
        {
            throw new InvalidCastException("Идентифиакатор сущности отсутствует или не найден");
        }

        if (!await accessChecker.CanEditAsync(attribute.EntityType, entityId, userId))
        {
            throw new ForbiddenAccessException(attribute.EntityType, entityId);
        }

        await next(context);
    }
}