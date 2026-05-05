using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Application.Interfaces;

public interface IDetalleServicio
{
    IEnumerable<Detalle> ObtenerTodos();
    IEnumerable<Detalle> ObtenerPorPrestamo(int prestamoId);
    Detalle? ObtenerPorId(int id);
    Result Crear(Detalle detalle);
    Result CrearMultiples(IEnumerable<Detalle> detalles);
    Result Actualizar(Detalle detalle);
    Result Eliminar(Detalle detalle);
}
