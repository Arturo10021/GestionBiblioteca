using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Domain.Ports;

public interface ILibroRepositorio : IRepository<Libro, int>
{
    IEnumerable<Autor> ObtenerNombresAutores();
    IEnumerable<Autor> ObtenerAutoresActivos();
    Dictionary<int, string> ObtenerTitulosLibros();
    bool ExisteAutorActivo(int autorId);
    int InsertarAutorYObtenerID(string nombreCompleto, int? usuarioSesionId);
}
