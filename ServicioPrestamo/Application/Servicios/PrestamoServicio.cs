using System;
using System.Collections.Generic;
using System.Linq;
using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Errors;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Application.Services;

public class PrestamoServicio : IPrestamoServicio
{
    private readonly PrestamoRepository _prestamoRepositorio;

    public PrestamoServicio(PrestamoRepository prestamoRepositorio)
    {
        _prestamoRepositorio = prestamoRepositorio;
    }

    public IEnumerable<Prestamo> Select(bool incluirInactivos = false)
    {
        return _prestamoRepositorio.GetAll(!incluirInactivos);
    }

    public Result Create(Prestamo prestamo)
    {
        var validacion = ValidarPrestamo(prestamo);
        if (validacion.IsFailure)
            return validacion;

        _prestamoRepositorio.Insert(prestamo);
        return Result.Success();
    }

    public Result Update(Prestamo prestamo)
    {
        var existente = _prestamoRepositorio.GetById(prestamo.PrestamoId);
        if (existente == null)
            return Result.Failure(new Error("Prestamo.NotFound", "Prestamo no encontrado"));

        if (prestamo.FechaDevolucionEsperada < prestamo.FechaPrestamo)
            return Result.Failure(PrestamoErrors.FechaDevolucionInvalida);

        _prestamoRepositorio.Update(prestamo);
        return Result.Success();
    }

    public Result Delete(Prestamo prestamo)
    {
        var existente = _prestamoRepositorio.GetById(prestamo.PrestamoId);
        if (existente == null)
            return Result.Failure(new Error("Prestamo.NotFound", "Prestamo no encontrado"));

        _prestamoRepositorio.Delete(prestamo);
        return Result.Success();
    }

    public Prestamo? GetById(int id) => _prestamoRepositorio.GetById(id);

    public Result ValidarPrestamo(Prestamo prestamo)
    {
        if (prestamo is null)
            return Result.Failure(PrestamoErrors.DatosObligatorios);

        if (prestamo.FechaDevolucionEsperada < prestamo.FechaPrestamo)
            return Result.Failure(PrestamoErrors.FechaDevolucionInvalida);

        return Result.Success();
    }

    public int CountPrestamosActivos(int lectorId)
    {
        var prestamos = _prestamoRepositorio.GetAll();
        return prestamos.Count(p => p.Estado == 1 && p.LectorId == lectorId);
    }

    public int InsertAndReturnId(Prestamo prestamo)
    {
        _prestamoRepositorio.Insert(prestamo);
        return prestamo.PrestamoId;
    }

    public int CrearPrestamoTransaccional(Prestamo prestamo, IEnumerable<Detalle> detalles, int? usuarioSesionId)
    {
        return _prestamoRepositorio.CrearPrestamoTransaccional(prestamo, detalles, usuarioSesionId);
    }
}
