using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Application.Interfaces;

public interface IEjemplarServicio
{
    IEnumerable<Ejemplar> Select();
    Result<Ejemplar> Create(Ejemplar dto);
    Result<Ejemplar> Update(Ejemplar dto);
    Result Delete(Ejemplar dto);
    Ejemplar? GetById(int id);

    Dictionary<int, string> ObtenerTitulosLibros();
    IEnumerable<Libro> ObtenerLibrosActivos();
    bool ExisteLibroActivo(int libroId);
    Dictionary<int, string> ObtenerEjemplaresDisponibles();

    Result ValidarEjemplar(Ejemplar ejemplar);
}
