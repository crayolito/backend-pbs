using System.Text.Json;
using BancoSol.Application.Exceptions;

namespace BancoSol.API.Middleware;

/**
* Middleware global para capturar excepciones no controladas
* Convierte errores internos en respuestas JSON uniformes para el cliente
*/
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    // Intercepta el pipeline HTTP y transforma excepciones en codigos correctos
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Ejecuta el siguiente middleware/controlador
            await _next(context);
        } catch (Exception ex)
        {
            // Registra el error completo para diagnostico interno
            _logger.LogError(ex, "Error no controlado en la solicitud {Path}", context.Request.Path);

            // Mapea excepcion a status code y mensaje seguro para el cliente
            var (statusCode, message) = MapException(ex);
            if (statusCode == StatusCodes.Status500InternalServerError && _environment.IsDevelopment())
            {
                message = $"{message}. Detalle: {ex.Message}";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var payload = JsonSerializer.Serialize(new { error = message });

            // Retorna siempre estructura JSON consistente
            await context.Response.WriteAsync(payload);
        }
    }

    // Traduce excepciones de negocio/tecnicas a respuestas HTTP estandar
    private static (int StatusCode, string Message) MapException(Exception ex)
    {
        return ex switch
        {
            BadRequestException => (StatusCodes.Status400BadRequest, ex.Message),
            NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "Ocurrio un error interno del servidor")
        };
    }
}