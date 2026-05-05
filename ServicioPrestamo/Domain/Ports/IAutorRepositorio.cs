using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Domain.Ports;

public interface IAutorRepositorio : IRepository<Autor, int>
{
    IEnumerable<Autor> ObtenerAutoresActivos();
    IEnumerable<Autor> ObtenerAutoresActivosTabla();
    bool ExisteAutorActivo(int autorId);
}
