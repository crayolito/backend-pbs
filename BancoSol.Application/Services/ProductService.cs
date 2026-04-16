using BancoSol.Application.DTOs;
using BancoSol.Application.Exceptions;
using BancoSol.Application.Interfaces;
using BancoSol.Domain.Entities;
using BancoSol.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BancoSol.Application.Services;

/// <summary>
/// Orquesta las cosas de uso de productos y aplica validaciones de negocio
/// </summary>
public sealed class ProductService : IProductService
{
    private const string ProductsListCacheKey = "products_list";
    private readonly IProductRepository _productRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ProductService> _logger;
    
    public ProductService(
        IProductRepository productRepository,
        IMemoryCache cache,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene una pagina de productos segun filtros y orden solicitados
    /// </summary>
    public async Task<PagedResultDto<ProductDto>> GetAllAsync(
        ProductQueryDto query,
        CancellationToken cancellationToken = default)
    {
        // Registra inicio de operación para trazabilidad operativa.
        _logger.LogInformation("Inicio GetAllAsync (page={Page}, size={Size})", query.Page, query.Size);

        // Valida reglas de negocio del rango de precios y paginacion
        ValidateQuery(query);

        // Si no hay filtro activos usa cache en memoriapor 5 minutos
        if (CanUseListCache(query) &&
            _cache.TryGetValue(ProductsListCacheKey, out PagedResultDto<ProductDto>? cached) &&
            cached is not null)
        {
            return cached;
        }

        // Delega acceso a datos al repositorio
        var (items, total) = await _productRepository.GetAllAsync(
            query.Search,
            query.Currency,
            query.InStock,
            query.MinPrice,
            query.MaxPrice,
            query.SortBy,
            query.Order,
            query.Page,
            query.Size,
            cancellationToken);

        // Construye respuesta paginada.
        var result = new PagedResultDto<ProductDto>
        {
            Data = items.Select(MapToDto).ToList(),
            Page = query.Page,
            Size = query.Size,
            Total = total
        };

        // Guarda en cache solo la lista completa sin filtros
        if (CanUseListCache(query))
        {
            _cache.Set(
                ProductsListCacheKey,
                result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }
        return result;
    }

    /// <summary>
    /// Actualiza el precio de un producto y devuelve el estado resultante
    /// </summary>
    public async Task<ProductDto> UpdatePriceAsync(
        Guid id,
        UpdatePriceDto request,
        CancellationToken cancellationToken = default
    ){
        // Registra inicio de operación para trazabilidad operativa.
        _logger.LogInformation("Inicio UpdatePriceAsync (id={ProductId})", id);
        
        // Valida reglas de negocio del precio y moneda
        ValidateUpdatePrice(request);

        // Delega acceso a datos al repositorio
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if(product is null)
        {
            // Producto inexistente: se registra como advertencia y se responde 404 vía middleware.
            _logger.LogWarning("Producto no encontrado. Id={ProductId}", id);
            throw new NotFoundException("Producto no encontrado.");
        }

        // Aplica cambio de precio y persiste
        product.Price = request.Price;
        product.Currency = request.Currency;
        await _productRepository.UpdateAsync(product, cancellationToken);

        // Invalida cache de lista completa después de un PATCH exitoso.
        _cache.Remove(ProductsListCacheKey);

        // Mapea entidad actualizada al DTO de respuesta.
        return MapToDto(product);
    }

    /// <summary>
    /// Valida filtros y paginacion para evitar combinaciones invalidas
    /// </summary>
    private static void ValidateQuery(ProductQueryDto query)
    {
        // Regla : minPrice no puede superar maxPrice
        if(query.MinPrice is not null && query.MaxPrice is not null && query.MinPrice > query.MaxPrice)
        {
            throw new BadRequestException("minPrice no puede ser mayor a maxPrice");
        }
    }

    /// <summary>
    /// Valida los datos del cambio de precio
    /// </summary>
    private static void ValidateUpdatePrice(UpdatePriceDto request)
    {
        // Regla : el precio debe ser mayor a cero
        if(request.Price <= 0)
        {
            throw new BadRequestException("El precio debe ser mayor a 0");
        }

        // Regla : la moneda no puede ser vacia
        if(string.IsNullOrWhiteSpace(request.Currency))
        {
            throw new BadRequestException("La moneda es obligatoria");
        }
    }

    /// <summary>
    /// Convierte una entidad de dominio a DTO de salida
    /// </summary>
    private static ProductDto MapToDto(Product product)
    {
        // Mapeo explicito para no exponer entidad en la capa API
        return new ProductDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Stock = product.Stock,
            Price = product.Price,
            Currency = product.Currency
        };
    }

    /// <summary>
    /// Determina si la consulta no tiene filtros para habilitar cache de lista completa.
    /// </summary>
    private static bool CanUseListCache(ProductQueryDto query)
    {
        // Normaliza orden por defecto: si no viene, se asume asc (comportamiento esperado del endpoint).
        var order = string.IsNullOrWhiteSpace(query.Order) ? "asc" : query.Order;

        return string.IsNullOrWhiteSpace(query.Search) &&
            string.IsNullOrWhiteSpace(query.Currency) &&
            query.InStock is null &&
            query.MinPrice is null &&
            query.MaxPrice is null &&
            string.IsNullOrWhiteSpace(query.SortBy) &&
            string.Equals(order, "asc", StringComparison.OrdinalIgnoreCase) &&
            query.Page == 1 &&
            query.Size == 10;
    }
}