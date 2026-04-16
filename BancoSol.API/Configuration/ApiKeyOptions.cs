namespace BancoSol.API.Configuration;

public sealed class ApiKeyOptions
{
    // Nombre estandar del header esperado en cada request
    public const string HeaderName = "X-Api-Key";

    // Valor de la API Key leido desde configuracion
    public string Value { get; set; } = string.Empty;
}