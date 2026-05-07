using Microsoft.AspNetCore.Mvc;
using ServicioPrestamo.Application.Fachadas;
using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamosController : ControllerBase
{
    private readonly IPrestamoFachada _prestamoFachada;
    private readonly IPrestamoServicio _prestamoServicio;
    private readonly IAnulacionFachada _anulacionFachada;

    public PrestamosController(
        IPrestamoFachada prestamoFachada,
        IPrestamoServicio prestamoServicio,
        IAnulacionFachada anulacionFachada)
    {
        _prestamoFachada = prestamoFachada;
        _prestamoServicio = prestamoServicio;
        _anulacionFachada = anulacionFachada;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Prestamo>> GetAll()
    {
        var prestamos = _prestamoServicio.Select();
        return Ok(prestamos);
    }

    [HttpGet("{id}")]
    public ActionResult<Prestamo> GetById(int id)
    {
        var prestamo = _prestamoFachada.ObtenerPrestamoPorId(id);
        return prestamo is null ? NotFound() : Ok(prestamo);
    }

    [HttpPost]
    public ActionResult Create([FromBody] CrearPrestamoRequest request)
    {
        var detalles = (request.Ejemplares ?? new List<CrearPrestamoEjemplarRequest>())
            .Select(e => (EjemplarId: e.EjemplarId, ObservacionesSalida: e.ObservacionesSalida));

        var result = _prestamoFachada.CrearPrestamoMultiple(
            request.LectorId,
            detalles,
            request.FechaDevolucionEsperada,
            request.UsuarioSesionId);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPost("{id}/anular")]
    public ActionResult Anular(int id, [FromBody] AnularPrestamoRequest request)
    {
        var result = _anulacionFachada.AnularPrestamo(
            id,
            request.UsuarioSesionId,
            request.Motivo);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return Ok(new { message = "Préstamo anulado correctamente." });
    }
}

public record CrearPrestamoRequest(
    int LectorId,
    List<CrearPrestamoEjemplarRequest> Ejemplares,
    DateTime FechaDevolucionEsperada,
    int? UsuarioSesionId
);

public record CrearPrestamoEjemplarRequest(
    int EjemplarId,
    string? ObservacionesSalida
);

public record AnularPrestamoRequest(
    int? UsuarioSesionId,
    string? Motivo
);
