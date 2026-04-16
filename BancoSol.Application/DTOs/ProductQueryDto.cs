using System.ComponentModel.DataAnnotations;

namespace BancoSol.Application.DTOs;

/// <summary>
/// Agrupamos los parametros de busqueda filtro orden paginacion
/// Se usa para mantener limpio el contrato del servicio
/// </summary>
public sealed class ProductQueryDto
{
    public string? Search { get; set; }

    [RegularExpression("^(BOB|USD|EUR)$", ErrorMessage = "Moneda debe ser BOB, USD o EUR")]
    public string? Currency { get; set; }

    public bool? InStock { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal? MinPrice { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal? MaxPrice { get; set; }

    [RegularExpression("^(price|name|stock)$", ErrorMessage = "SortBy debe ser price, name o stock")]
    public string? SortBy { get; set; }

    [RegularExpression("^(asc|desc)$", ErrorMessage = "Order debe ser asc o desc")]
    public string? Order { get; set; } = "asc";

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1,200)]
    public int Size { get; set; } = 10;
}