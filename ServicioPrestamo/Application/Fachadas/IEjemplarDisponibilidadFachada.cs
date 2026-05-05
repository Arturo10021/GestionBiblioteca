using ServicioPrestamo.Domain.Common;

namespace ServicioPrestamo.Application.Fachadas;

public interface IEjemplarDisponibilidadFachada
{
    Result CambiarDisponibilidad(int ejemplarId, bool disponible, int? usuarioSesionId = null);
}