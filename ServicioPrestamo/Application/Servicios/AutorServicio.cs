using System.Collections.Generic;
using System.Linq;
using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Errors;
using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Infrastructure.Formatting;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Application.Services;

public class AutorServicio : IAutorServicio
{
    private readonly AutorRepository _autorRepositorio;

    public AutorServicio(AutorRepository autorRepositorio)
    {
        _autorRepositorio = autorRepositorio;
    }

    public IEnumerable<Autor> Select(bool incluirInactivos = false)
    {
        var autores = _autorRepositorio.GetAll(!incluirInactivos);
        return autores.Select(a => new Autor
        {
            AutorId = a.AutorId,
            Nombres = a.Nombres.ToDisplayName(),
            Apellidos = a.Apellidos.ToDisplayName(),
            Nacionalidad = a.Nacionalidad,
            FechaNacimiento = a.FechaNacimiento,
            Estado = a.Estado,
            RouteToken = a.RouteToken
        });
    }

    public Result<Autor> Create(Autor Autor)
    {
        if (string.IsNullOrWhiteSpace(Autor.Nombres))
        {
            return Result<Autor>.Failure(AutorErrors.NombresObligatorios);
        }

        var autor = new Autor
        {
            Nombres = Autor.Nombres.ToDisplayName(),
            Apellidos = Autor.Apellidos.ToDisplayName(),
            Nacionalidad = Autor.Nacionalidad,
            FechaNacimiento = Autor.FechaNacimiento,
            Estado = Autor.Estado,
            UsuarioSesionId = Autor.UsuarioSesionId,
            RouteToken = Guid.NewGuid().ToString("N"),
            FechaRegistro = DateTime.UtcNow
        };

        _autorRepositorio.Insert(autor);
        
        Autor.AutorId = autor.AutorId;
        Autor.Nombres = autor.Nombres;
        Autor.Apellidos = autor.Apellidos;
        return Result<Autor>.Success(Autor);
    }

    public Result<Autor> Update(Autor Autor)
    {
        var autorExistente = _autorRepositorio.GetById(Autor.AutorId);
        if (autorExistente == null)
        {
            return Result<Autor>.Failure(AutorErrors.AutorNoEncontrado);
        }

        if (string.IsNullOrWhiteSpace(Autor.Nombres))
        {
            return Result<Autor>.Failure(AutorErrors.NombresObligatorios);
        }

        autorExistente.UsuarioSesionId = Autor.UsuarioSesionId;
        autorExistente.Nombres = Autor.Nombres.ToDisplayName();
        autorExistente.Apellidos = Autor.Apellidos.ToDisplayName();
        autorExistente.Nacionalidad = Autor.Nacionalidad;
        autorExistente.FechaNacimiento = Autor.FechaNacimiento;
        autorExistente.Estado = Autor.Estado;
        autorExistente.UsuarioSesionId = Autor.UsuarioSesionId;
        autorExistente.UltimaActualizacion = DateTime.UtcNow;

        _autorRepositorio.Update(autorExistente);
        Autor.Nombres = autorExistente.Nombres;
        Autor.Apellidos = autorExistente.Apellidos;
        return Result<Autor>.Success(Autor);
    }

    public Result Delete(int autorId, int? usuarioSesionId)
    {
        var autor = _autorRepositorio.GetById(autorId);
        if (autor == null)
            return Result.Failure(AutorErrors.AutorNoEncontrado);

        autor.UsuarioSesionId = usuarioSesionId;
        _autorRepositorio.Delete(autor);
        return Result.Success();
    }

    public Autor? GetById(int id)
    {
        var a = _autorRepositorio.GetById(id);
        if (a == null) return null;

        return new Autor
        {
            AutorId = a.AutorId,
            Nombres = a.Nombres.ToDisplayName(),
            Apellidos = a.Apellidos.ToDisplayName(),
            Nacionalidad = a.Nacionalidad,
            FechaNacimiento = a.FechaNacimiento,
            Estado = a.Estado,
            RouteToken = a.RouteToken
        };
    }

    public Dictionary<int, string> ObtenerAutoresActivos() 
    {
        var dict = new Dictionary<int, string>();
        var autores = _autorRepositorio.ObtenerAutoresActivos();
        foreach(var a in autores)
        {
            dict[a.AutorId] = a.Nombres.ToDisplayName();
        }
        return dict;
    }

    public bool ExisteAutorActivo(int autorId) => _autorRepositorio.ExisteAutorActivo(autorId);
}
