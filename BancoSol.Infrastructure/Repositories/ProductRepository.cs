using BancoSol.Domain.Entities;
using BancoSol.Domain.Interfaces;
using BancoSol.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoSol.Infrastructure.Repositories;

/// <summary>
/// Implementa acceso a datos de productos usando EF Core
/// Mantiene consultas y persistencia fuera de la capa de negocio
/// </summary>
public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyCollection<Product> Items, int Total)> GetAllAsync(
        string? search,
        string? currency,
        bool? inStock,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        string? order,
        int page,
        int size,
        CancellationToken cancellationToken = default)
    {
        // Fase 1 : Contruye query base de solo lectura
        IQueryable<Product> query = _context.Products.AsNoTracking();

        // Fase 2 : Aplica filtros de opcionales
        query = ApplyFilters(query, search, currency, inStock, minPrice, maxPrice);

        // Fase 3 : Aplica ordenamiento solicitado
        query = ApplySorting(query, sortBy, order);

        // Fase 4 : Calcula total antes de paginar
        var total = await query.CountAsync(cancellationToken);

        // Fase 5 : Aplicar paginacion y materializa resultados
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return (items, total);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Busca con tracking para permitir Update/SaveChanges en PATCH sin conflictos de estado.
        return _context.Products.AsTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    
    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        // Marca entidad como modificada y persiste cambios
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Product> ApplyFilters(
        IQueryable<Product> query,
        string? search,
        string? currency,
        bool? inStock,
        decimal? minPrice,
        decimal? maxPrice
    ){
        if(!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term) || p.Sku.ToLower().Contains(term));
        }

        if(!string.IsNullOrWhiteSpace(currency))
        {
            query = query.Where(p => p.Currency == currency);
        }

        if (inStock == true)
        {
            query = query.Where(p => p.Stock > 0);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        return query;
    }

    private static IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        string? sortBy,
        string? order
    ){
        var isDesc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);

        return (sortBy?.ToLower(), isDesc) switch
        {
            ("price", true) => query.OrderByDescending(p => p.Price),
            ("price", false) => query.OrderBy(p => p.Price),
            ("stock", true) => query.OrderByDescending(p => p.Stock),
            ("stock", false) => query.OrderBy(p => p.Stock),
            ("name", true) => query.OrderByDescending(p => p.Name),
            _ => query.OrderBy(p => p.Name)
        };
    }
}