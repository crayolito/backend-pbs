using BancoSol.Application.DTOs;

namespace BancoSol.Application.Interfaces;

/// <summary>
/// Expone los casos de uso de productos para la capa API.
/// La validación de reglas de negocio vive en la implementación del servicio.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Obtiene productos aplicando filtros, ordenamiento y paginación.
    /// </summary>
    Task<PagedResultDto<ProductDto>> GetAllAsync(
        ProductQueryDto query,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Actualiza el precio de un producto existente.
    /// </summary>
    Task<ProductDto> UpdatePriceAsync(
        Guid id,
        UpdatePriceDto request,
        CancellationToken cancellationToken = default
    );
}