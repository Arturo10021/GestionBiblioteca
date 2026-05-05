using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Ports;
using System.Collections.Generic;
using System.Linq;

namespace ServicioPrestamo.Application.Fachadas;

public class PrestamoFachada : IPrestamoFachada
{
    private readonly IPrestamoServicio _prestamoServicio;
    private readonly IEjemplarServicio _ejemplarServicio;
    private readonly IDetalleServicio _detalleServicio;
    private readonly IEjemplarDisponibilidadFachada _disponibilidadFachada;

    public PrestamoFachada(IPrestamoServicio prestamoServicio, IEjemplarServicio ejemplarServicio, IDetalleServicio detalleServicio, IEjemplarDisponibilidadFachada disponibilidadFachada)
    {
        _prestamoServicio = prestamoServicio;
        _ejemplarServicio = ejemplarServicio;
        _detalleServicio = detalleServicio;
        _disponibilidadFachada = disponibilidadFachada;
    }

    public IEnumerable<KeyValuePair<int, string>> BuscarEjemplaresActivos(string q)
    {
        var disponibles = _ejemplarServicio.ObtenerEjemplaresDisponibles();
        if (string.IsNullOrWhiteSpace(q))
            return disponibles;

        var lower = q.ToLowerInvariant();
        return disponibles.Where(kv => kv.Value.ToLowerInvariant().Contains(lower));
    }

    public Result<int> CrearPrestamoMultiple(int lectorId, IEnumerable<int> ejemplarIds, DateTime fechaDevolucionEsperada, int? usuarioSesionId = null, string? observacionesSalida = null)
    {
        var detallesEjemplares = (ejemplarIds ?? Enumerable.Empty<int>())
            .Select(id => (EjemplarId: id, ObservacionesSalida: observacionesSalida));
        return CrearPrestamoMultiple(lectorId, detallesEjemplares, fechaDevolucionEsperada, usuarioSesionId);
    }

    public Result<int> CrearPrestamoMultiple(int lectorId, IEnumerable<(int EjemplarId, string? ObservacionesSalida)> detallesEjemplares, DateTime fechaDevolucionEsperada, int? usuarioSesionId = null)
    {
        var detallesEntrada = detallesEjemplares?.ToList() ?? new List<(int EjemplarId, string? ObservacionesSalida)>();
        var ejemplares = detallesEntrada.Select(x => x.EjemplarId).ToList();

        if (!ejemplares.Any())
            return Result<int>.Failure(new Error("Prestamo.Error", "Debes seleccionar al menos un ejemplar."));

        if (ejemplares.Count > 5)
            return Result<int>.Failure(new Error("Prestamo.Error", "No se pueden prestar más de 5 ejemplares a la vez."));

        var actuales = _prestamoServicio.CountPrestamosActivos(lectorId);
        if (actuales >= 5)
            return Result<int>.Failure(new Error("Prestamo.Limite", "El lector ya tiene el máximo de préstamos activos (5)."));

        foreach (var ejemplarId in ejemplares)
        {
            var ejemplar = _ejemplarServicio.GetById(ejemplarId);
            if (ejemplar == null || !ejemplar.Disponible)
                return Result<int>.Failure(new Error("Prestamo.Error", $"El ejemplar {ejemplarId} no está disponible."));
        }

        try
        {
            var prestamo = new Prestamo
            {
                LectorId = lectorId,
                FechaPrestamo = DateTime.Now,
                FechaDevolucionEsperada = fechaDevolucionEsperada,
                ObservacionesSalida = detallesEntrada.FirstOrDefault().ObservacionesSalida,
                Estado = 1,
                UsuarioSesionId = usuarioSesionId
            };

            _prestamoServicio.InsertAndReturnId(prestamo);
            if (prestamo.PrestamoId <= 0)
                return Result<int>.Failure(new Error("Prestamo.Error", "No se pudo obtener el ID del préstamo."));

            var detalles = new List<Detalle>();
            foreach (var item in detallesEntrada)
            {
                detalles.Add(new Detalle
                {
                    PrestamoId = prestamo.PrestamoId,
                    EjemplarId = item.EjemplarId,
                    EstadoDetalle = 1,
                    ObservacionesSalida = item.ObservacionesSalida,
                    UsuarioSesionId = usuarioSesionId,
                    FechaRegistro = DateTime.Now
                });
            }

            var resultadoDetalles = _detalleServicio.CrearMultiples(detalles);
            if (resultadoDetalles.IsFailure)
                return Result<int>.Failure(resultadoDetalles.Error);

            foreach (var ejemplarId in ejemplares)
            {
                var result = _disponibilidadFachada.CambiarDisponibilidad(ejemplarId, false, usuarioSesionId);
                if (result.IsFailure)
                    return Result<int>.Failure(result.Error);
            }

            return Result<int>.Success(prestamo.PrestamoId);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(new Error("Prestamo.Error", $"Error al crear préstamo: {ex.Message}"));
        }
    }

    public int CountPrestamosActivos(int lectorId) => _prestamoServicio.CountPrestamosActivos(lectorId);

    public Prestamo? ObtenerPrestamoPorId(int id) => _prestamoServicio.GetById(id);

    public Ejemplar? ObtenerEjemplarPorId(int id)
    {
        var dto = _ejemplarServicio.GetById(id);
        if (dto == null) return null;
        return new Ejemplar
        {
            EjemplarId = dto.EjemplarId,
            CodigoInventario = dto.CodigoInventario,
            EstadoConservacion = dto.EstadoConservacion,
            Disponible = dto.Disponible,
            MotivoBaja = dto.MotivoBaja
        };
    }

    public string? ObtenerLabelEjemplar(int ejemplarId)
    {
        var dto = _ejemplarServicio.GetById(ejemplarId);
        if (dto == null) return null;
        var titulos = _ejemplarServicio.ObtenerTitulosLibros();
        if (titulos.TryGetValue(dto.LibroId, out var titulo))
            return $"{titulo} ({dto.CodigoInventario})";
        return null;
    }
}
