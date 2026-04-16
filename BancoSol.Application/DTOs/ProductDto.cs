using System.ComponentModel.DataAnnotations;

namespace BancoSol.Application.DTOs;

/// <summary>
/// Representa el producto expuesto por el API 
/// Exista para no devolver la entidad de dominio directamente
/// </summary>
public sealed class ProductDto 
{
    public Guid Id { get; set; }

    [Required]
    public string Sku { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public int Stock { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal Price { get; set; }

    [Required]
    [RegularExpression("^(BOB|USD|EUR)$", ErrorMessage = "Currency must be BOB, USD or EUR")]
    public string Currency { get; set; } = string.Empty;
}