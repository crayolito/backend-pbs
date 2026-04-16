/** 
* Excepcion de negocio para recursos no encontrados (HTTP 404)
* Se usa cuando el producto solicitado no existe en la base de datos
*/

namespace BancoSol.Application.Exceptions;

// Representa errores de recurso inexistente que deben responder 404
public sealed class NotFoundException : Exception
{
    // Inicializa la excepcion con un mensaje claro para el cliente
    public NotFoundException(string message) : base(message) {}
}