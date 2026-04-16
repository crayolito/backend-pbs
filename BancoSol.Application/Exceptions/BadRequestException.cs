/** 
* Excepcion de negocio para solicitudes invalidas  (HTTP 400)
* Se usa ucando los datos de entrada no cumple las reglas funcionales
*/

namespace BancoSol.Application.Exceptions;

// Representa errores de validacion de negocio que deben responder 400
public sealed class BadRequestException : Exception
{
    // Inicializa la excepcion con un mensaje claro para el cliente
    public BadRequestException(string message) : base(message) {}
}