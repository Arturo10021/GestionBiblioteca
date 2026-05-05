using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;
using System.Collections.Generic;

namespace ServicioPrestamo.Application.Fachadas;

public interface IPrestamoFachada
{
    IEnumerable<KeyValuePair<int, string>> BuscarEjemplaresActivos(string q);
    Result<int> CrearPrestamoMultiple(int lectorId, IEnumerable<int> ejemplarIds, DateTime fechaDevolucionEsperada, int? usuarioSesionId = null, string? observacionesSalida = null);
    Result<int> CrearPrestamoMultiple(int lectorId, IEnumerable<(int EjemplarId, string? ObservacionesSalida)> detallesEjemplares, DateTime fechaDevolucionEsperada, int? usuarioSesionId = null);
    int CountPrestamosActivos(int lectorId);
    Prestamo? ObtenerPrestamoPorId(int id);
    Ejemplar? ObtenerEjemplarPorId(int id);
    string? ObtenerLabelEjemplar(int ejemplarId);
}
