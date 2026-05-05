using Frontend.Dtos;
using Frontend.Helpers;
using Frontend.Dtos;

namespace Frontend.Adapters;

public interface IEjemplarServicio
{
    IEnumerable<EjemplarDto> Select();
    Result<EjemplarDto> Create(EjemplarDto dto);
    Result<EjemplarDto> Update(EjemplarDto dto);
    Result Delete(EjemplarDto dto);
    EjemplarDto? GetById(int id);
    Dictionary<int, string> ObtenerTitulosLibros();
    IEnumerable<LibroDto> ObtenerLibrosActivos();
    bool ExisteLibroActivo(int libroId);
    Dictionary<int, string> ObtenerEjemplaresDisponibles();
    Result ValidarEjemplar(EjemplarDto EjemplarDto);
}
