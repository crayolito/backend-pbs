using BancoSol.API.Configuration;
using BancoSol.Application.Interfaces;
using BancoSol.Application.Services;
using BancoSol.Domain.Interfaces;
using BancoSol.Infrastructure.Data;
using BancoSol.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace BancoSol.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        // Unifica respuesta de errores de validacion a { "error": "mensaje" }
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var firstError = context.ModelState
                    .Where(kvp => kvp.Value is not null && kvp.Value.Errors.Count > 0)
                    .Select(kvp => kvp.Value!.Errors.First().ErrorMessage)
                    .FirstOrDefault();

                var message = string.IsNullOrWhiteSpace(firstError)
                    ? "Solicitud invalida"
                    : firstError;

                return new BadRequestObjectResult(new { error = message });
            };
        });

        // Habilita cache en memoria para escenearios de lectura frecuente
        services.AddMemoryCache();

        // Vincula la API Key desde configuracion
        services.Configure<ApiKeyOptions>(options =>
        {
            options.Value = configuration["ApiKey"] ?? string.Empty;
        });

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        // Configura Swagger mas seguridad por API Key
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BancoSol API", Version = "v1" });

            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Name = ApiKeyOptions.HeaderName,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Description = "Ingresa tu API Key",
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("ApiKey", document, null!)] = []
            });
        });

        return services;
    }

    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Configura EF Core con SQLite
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=products.db";

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        // Registro de servicios de aplicacion e infraestructura
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}