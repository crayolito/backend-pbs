using BancoSol.API.Extensions;
using BancoSol.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((_, _, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day);
});

// Servicios
builder.Services
    .AddApiCore(builder.Configuration)
    .AddSwaggerDocumentation()
    .AddApplicationDependencies(builder.Configuration);

var app = builder.Build();

// Infraestructura inicial
await app.InitializeDatabaseAsync();

// Pipeline HTTP
app.UseApiDocumentation();
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();