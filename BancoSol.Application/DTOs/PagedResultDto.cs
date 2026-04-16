namespace BancoSol.Application.DTOs;

/// <summary>
/// Estandariza la respuesta paginada solicitada por al regla del proyecto
/// </summary>
public sealed class PagedResultDto<T>
{
    public IReadOnlyCollection<T> Data { get; set; } = Array.Empty<T>();
    public int Page { get; set; }
    public int Size { get; set; }
    public int Total { get; set; }
}

