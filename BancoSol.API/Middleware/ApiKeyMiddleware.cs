using BancoSol.API.Configuration;
using Microsoft.Extensions.Options;

namespace BancoSol.API.Middleware;

public sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiKeyOptions _options;

    public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Permite acceder a Swagger/OpenAPI sin API Key para documentacion :3
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/openapi"))
        {
            await _next(context);
            return;
        }

        // Si no hay API Key configurado responde error de servidor
        if(string.IsNullOrEmpty(_options.Value))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "API Key no configurada en appsettings.json." });
            return;
        }

        // Valida existencia y valor correcto del header X-Api-Key
        var hasHeader = context.Request.Headers.TryGetValue(ApiKeyOptions.HeaderName, out var headerValue);

        if (!hasHeader || !string.Equals(headerValue.ToString(), _options.Value, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API Key invalida o ausente." });
            return;
        }

        await _next(context);
    }
}