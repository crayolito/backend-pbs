using BancoSol.Application.DTOs;
using BancoSol.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace BancoSol.API.Controllers;

/**
* Controller de productos
* Solo recive requests HTTP delega al servicio y devuelve respuestas
*/
[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // Obtiene productos con filtros, orden y paginacion desde query string
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] ProductQueryDto query,
        CancellationToken cancellationToken = default
    ){
        // Delega reglas de negocio al servicio de aplicacion
        var result = await _productService.GetAllAsync(query, cancellationToken);

        // Retorna 200 con formato paginado
        return Ok(result);
    }

    // Actualiza precio/moneda de un producto por id
    [HttpPatch("{id:guid}/price")]
    public async Task<IActionResult> UpdatePriceAsync(
        Guid id,
        [FromBody] UpdatePriceDto request,
        CancellationToken cancellationToken)
    {
        // Delega validacion y persistencia al servicio
        var result = await _productService.UpdatePriceAsync(id, request, cancellationToken);

        // Retorna 200 con el producto actualizado
        return Ok(result);
    }
}