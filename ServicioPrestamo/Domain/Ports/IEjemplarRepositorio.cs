using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Domain.Ports;

public interface IEjemplarRepositorio : IRepository<Ejemplar, int>
{
    Dictionary<int, string> ObtenerTitulosLibros();
    IEnumerable<Libro> ObtenerLibrosActivos();
    bool ExisteLibroActivo(int libroId);
    Dictionary<int, string> ObtenerEjemplaresDisponibles();
}
