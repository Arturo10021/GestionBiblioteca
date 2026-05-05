using System.Collections.Generic;
using System.Linq;
using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Errors;
using ServicioPrestamo.Domain.Ports;
using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Application.Services;

public class AutorServicio : IAutorServicio
{
    private readonly IAutorRepositorio _autorRepositorio;

    public AutorServicio(IAutorRepositorio autorRepositorio)
    {
        _autorRepositorio = autorRepositorio;
    }

    public IEnumerable<Autor> Select() 
    {
        var autores = _autorRepositorio.GetAll();
        return autores.Select(a => new Autor
        {
            AutorId = a.AutorId,
            Nombres = a.Nombres,
            Apellidos = a.Apellidos,
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
            Nombres = Autor.Nombres,
            Apellidos = Autor.Apellidos,
            Nacionalidad = Autor.Nacionalidad,
            FechaNacimiento = Autor.FechaNacimiento,
            Estado = Autor.Estado,
            RouteToken = Guid.NewGuid().ToString("N"),
            FechaRegistro = DateTime.UtcNow
        };

        _autorRepositorio.Insert(autor);
        
        Autor.AutorId = autor.AutorId;
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

        autorExistente.Nombres = Autor.Nombres;
        autorExistente.Apellidos = Autor.Apellidos;
        autorExistente.Nacionalidad = Autor.Nacionalidad;
        autorExistente.FechaNacimiento = Autor.FechaNacimiento;
        autorExistente.Estado = Autor.Estado;
        autorExistente.UltimaActualizacion = DateTime.UtcNow;

        _autorRepositorio.Update(autorExistente);
        return Result<Autor>.Success(Autor);
    }

    public Result Delete(int autorId)
    {
        var autor = _autorRepositorio.GetById(autorId);
        if (autor == null)
            return Result.Failure(AutorErrors.AutorNoEncontrado);
            
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
            Nombres = a.Nombres,
            Apellidos = a.Apellidos,
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
            dict[a.AutorId] = a.Nombres;
        }
        return dict;
    }

    public bool ExisteAutorActivo(int autorId) => _autorRepositorio.ExisteAutorActivo(autorId);
}
