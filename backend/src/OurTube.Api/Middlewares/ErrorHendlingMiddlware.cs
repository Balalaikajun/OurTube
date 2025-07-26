using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using OurTube.Application.DTOs.Common;
using OurTube.Domain.Exceptions;

namespace OurTube.Api.Middlewares;

public class ErrorHendlingMiddlware(RequestDelegate next)
{
    
    
    public async Task InvokeAsync(HttpContext context, ILogger<ErrorHendlingMiddlware> logger)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            
            var error = MapException(ex);

            context.Response.ContentType = "application/json";
            
            var json = JsonSerializer.Serialize(error);

            await context.Response.WriteAsync(json);
        }
    }

    private static Error MapException(Exception ex)
    {
        return ex switch
        {
            NotFoundException => new Error(StatusCodes.Status404NotFound.ToString(), StatusCodes.Status404NotFound,
                ex.Message),
            InvalidOperationException => new Error(StatusCodes.Status400BadRequest.ToString(), StatusCodes.Status400BadRequest, ex.Message ),
            UnauthorizedAccessException => new Error(StatusCodes.Status403Forbidden.ToString(), StatusCodes.Status403Forbidden, ex.Message),
            ArgumentException => new Error(StatusCodes.Status400BadRequest.ToString(), StatusCodes.Status400BadRequest, ex.Message ),
            _ => new Error(StatusCodes.Status500InternalServerError.ToString(), StatusCodes.Status500InternalServerError, ex.Message)
        };
    }
}