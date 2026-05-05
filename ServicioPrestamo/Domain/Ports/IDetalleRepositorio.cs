using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Domain.Ports;

public interface IDetalleRepositorio : IRepository<Detalle, int>
{
    IEnumerable<Detalle> GetByPrestamoId(int prestamoId);
    void InsertMany(IEnumerable<Detalle> detalles);
}
