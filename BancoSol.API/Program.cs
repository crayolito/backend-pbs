using BancoSol.API.Extensions;
using BancoSol.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuracion SeriLog para Loggin en consola y archivo diario
builder.Host.UseSerilog((_, _, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day);
});

// Registra servicios base swagger y dependencias de aplicacion y infraestructura
builder.Services
    .AddApiCore(builder.Configuration)
    .AddSwaggerDocumentation()
    .AddApplicationDependencies(builder.Configuration);

var app = builder.Build();

// Inicializa la base de datos al arracar la API
await app.InitializeDatabaseAsync();

// Configurar el pipeline HTTP
app.UseApiDocumentation();
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();