using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Domain.Ports;

public interface IPrestamoRepositorio : IRepository<Prestamo, int>
{
    new int Insert(Prestamo prestamo);
    void InsertManyWithTransaction(IEnumerable<Prestamo> prestamos);
}
