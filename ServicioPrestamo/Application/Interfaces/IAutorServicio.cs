using System.Collections.Generic;
using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Application.Interfaces;

public interface IAutorServicio
{
    IEnumerable<Autor> Select();
    Result<Autor> Create(Autor Autor);
    Result<Autor> Update(Autor Autor);
    Result Delete(int autorId);
    Autor? GetById(int id);

    Dictionary<int, string> ObtenerAutoresActivos();
    bool ExisteAutorActivo(int autorId);
}
