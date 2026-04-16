using System.ComponentModel.DataAnnotations;

namespace BancoSol.Application.DTOs;

/// <summary>
/// Define el payload para actualziar solo el precio del producto
/// </summary>
public sealed class UpdatePriceDto
{
    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal Price { get; set; }

    [RegularExpression("^(BOB|USD|EUR)$", ErrorMessage = "Currency must be BOB, USD or EUR")]
    public string Currency { get; set; } = string.Empty;
}

