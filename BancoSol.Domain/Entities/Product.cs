namespace BancoSol.Domain.Entities;

/// <summary>
/// Representa el agregado principal de productos en dominio
/// Esta entidad contiene solo estado y no depende de capas externas
/// </summary>
public sealed class Product
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
}