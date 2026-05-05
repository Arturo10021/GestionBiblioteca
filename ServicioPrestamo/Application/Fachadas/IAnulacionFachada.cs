using ServicioPrestamo.Domain.Common;

namespace ServicioPrestamo.Application.Fachadas;

public interface IAnulacionFachada
{
    Result AnularPrestamo(int prestamoId, int? usuarioSesionId = null, string? motivo = null);
}