using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Validations;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Application.Services;

public class DetalleServicio : IDetalleServicio
{
    private readonly DetalleRepository _detalleRepositorio;

    public DetalleServicio(DetalleRepository detalleRepositorio)
    {
        _detalleRepositorio = detalleRepositorio;
    }

    public IEnumerable<Detalle> ObtenerTodos()
    {
        return _detalleRepositorio.GetAll();
    }

    public IEnumerable<Detalle> ObtenerPorPrestamo(int prestamoId)
    {
        return _detalleRepositorio.GetByPrestamoId(prestamoId);
    }

    public Detalle? ObtenerPorId(int id)
    {
        return _detalleRepositorio.GetById(id);
    }

    public Result Crear(Detalle detalle)
    {
        try
        {
            ValidarDetalle(detalle);
            _detalleRepositorio.Insert(detalle);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Detalle.Error", ex.Message));
        }
    }

    public Result CrearMultiples(IEnumerable<Detalle> detalles)
    {
        try
        {
            foreach (var detalle in detalles)
            {
                ValidarDetalle(detalle);
            }

            _detalleRepositorio.InsertMany(detalles);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Detalle.Error", ex.Message));
        }
    }

    public Result Actualizar(Detalle detalle)
    {
        try
        {
            ValidarDetalle(detalle);
            _detalleRepositorio.Update(detalle);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Detalle.Error", ex.Message));
        }
    }

    public Result Eliminar(Detalle detalle)
    {
        try
        {
            _detalleRepositorio.Delete(detalle);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Detalle.Error", ex.Message));
        }
    }

    private void ValidarDetalle(Detalle detalle)
    {
        if (detalle.PrestamoId <= 0)
            throw new ArgumentException("PrestamoId debe ser mayor a 0");

        if (detalle.EjemplarId <= 0)
            throw new ArgumentException("EjemplarId debe ser mayor a 0");

        if (detalle.EstadoDetalle < 0 || detalle.EstadoDetalle > 2)
            throw new ArgumentException("EstadoDetalle debe estar entre 0 y 2");

        detalle.ObservacionesSalida = NormalizarYValidarObservacion(detalle.ObservacionesSalida, "observación de salida");
        detalle.ObservacionesEntrada = NormalizarYValidarObservacion(detalle.ObservacionesEntrada, "observación de entrada");
    }

    private static string? NormalizarYValidarObservacion(string? observacion, string nombreCampo)
    {
        if (string.IsNullOrWhiteSpace(observacion))
            return null;

        var normalizada = ValidadorEntrada.NormalizarEspacios(observacion);

        if (ValidadorEntrada.ExcedeLongitud(normalizada, 150))
            throw new ArgumentException($"La {nombreCampo} no puede exceder 150 caracteres.");

        if (!ValidadorEntrada.SoloLetrasYNumeros(normalizada))
            throw new ArgumentException($"La {nombreCampo} solo permite letras, números y espacios.");

        return normalizada;
    }
}
