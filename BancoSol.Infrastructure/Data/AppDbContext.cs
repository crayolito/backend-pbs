using BancoSol.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoSol.Infrastructure.Data;

/// <summary>
/// Contexto EF Core de la aplicación.
/// Centraliza mapeos y datos semilla de productos.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configura precisión monetaria para evitar redondeos inesperados
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        // Carga datos iniciales 
        modelBuilder.Entity<Product>().HasData(SeedData.Products);

        base.OnModelCreating(modelBuilder);
    }
}