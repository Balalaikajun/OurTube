namespace OurTube.Api.Middlewares;

public class UniqueVisitorId
{
    private readonly RequestDelegate _next;

    private const string SessionIdCookieField = "SessionId";
    
    public UniqueVisitorId(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Cookies.ContainsKey(SessionIdCookieField))
        {
            var anonId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append(SessionIdCookieField, anonId, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                IsEssential = true
            });
        }

        await _next(context);
    }
}