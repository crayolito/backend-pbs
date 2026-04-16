using BancoSol.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoSol.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        // Exponer Swagger en dev
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BancoSol API v1");
                c.RoutePrefix = string.Empty;
                c.ConfigObject.PersistAuthorization = true;
            });
        }

        return app;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        // Crea base de datos en primer arranque si no existe
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}