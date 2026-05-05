using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Application.Interfaces;

public interface ILibroServicio
{
    IEnumerable<Libro> Select();
    Libro? GetById(int id);

    Result Create(Libro Libro, string? nombreAutorNuevo);
    Result Update(Libro Libro);
    Result Delete(int libroId, int? usuarioSesionId);

    Dictionary<int, string> ObtenerNombresAutores();
    IEnumerable<Autor> ObtenerAutoresActivos();
    bool ExisteAutorActivo(int autorId);
    int InsertarAutorYObtenerID(string nombreCompleto, int? usuarioSesionId);
}
