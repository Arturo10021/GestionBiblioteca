using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Errors;

namespace ServicioPrestamo.Application.Fachadas;

public class EjemplarDisponibilidadFachada : IEjemplarDisponibilidadFachada
{
    private readonly IEjemplarServicio _ejemplarServicio;

    public EjemplarDisponibilidadFachada(IEjemplarServicio ejemplarServicio)
    {
        _ejemplarServicio = ejemplarServicio;
    }

    public Result CambiarDisponibilidad(int ejemplarId, bool disponible, int? usuarioSesionId = null)
    {
        if (ejemplarId <= 0)
            return Result.Failure(new Error("Ejemplar.Error", "Id inválido"));

        var ejemplar = _ejemplarServicio.GetById(ejemplarId);
        if (ejemplar == null)
            return Result.Failure(new Error("Ejemplar.NotFound", "Ejemplar no encontrado"));

        
        if (ejemplar.Disponible == disponible)
            return Result.Success();

        ejemplar.Disponible = disponible;
        ejemplar.UsuarioSesionId = usuarioSesionId;

        return _ejemplarServicio.Update(ejemplar);
    }
}