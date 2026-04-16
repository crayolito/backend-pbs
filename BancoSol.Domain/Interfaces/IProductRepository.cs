using BancoSol.Domain.Entities;

namespace BancoSol.Domain.Interfaces;

/// <summary>
/// Define contrato de persistencia para productos
/// El dominio conoce el contrato pero no detalles de EF ni SQL
/// </summary>
public interface IProductRepository
{
    Task<(IReadOnlyCollection<Product> Items, int Total)> GetAllAsync(
        string? search,
        string? currency,
        bool? inStock,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        string? order,
        int page,
        int size,
        CancellationToken cancellationToken = default);
    
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
}