using Frontend.Dtos;
using Frontend.Helpers;

namespace Frontend.Adapters;

public interface IAutorServicio
{
    IEnumerable<AutorDto> Select();
    Result<AutorDto> Create(AutorDto dto);
    Result<AutorDto> Update(AutorDto dto);
    Result Delete(int id);
    AutorDto? GetById(int id);
    Dictionary<int, string> ObtenerAutoresActivos();
    bool ExisteAutorActivo(int autorId);
}
