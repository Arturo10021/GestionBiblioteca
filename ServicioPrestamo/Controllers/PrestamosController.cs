using Microsoft.AspNetCore.Mvc;
using ServicioPrestamo.Application.Fachadas;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamosController : ControllerBase
{
    private readonly IPrestamoFachada _prestamoFachada;

    public PrestamosController(IPrestamoFachada prestamoFachada)
    {
        _prestamoFachada = prestamoFachada;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Prestamo>> GetAll()
    {
        // Simple list from fachada via servicio
        return Ok(Enumerable.Empty<Prestamo>()); // TODO: add GetAll to fachada
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
        var result = _prestamoFachada.CrearPrestamoMultiple(
            request.LectorId,
            request.EjemplarIds,
            request.FechaDevolucionEsperada,
            request.UsuarioSesionId,
            request.Observaciones);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, null);
    }
}

public record CrearPrestamoRequest(
    int LectorId,
    List<int> EjemplarIds,
    DateTime FechaDevolucionEsperada,
    int? UsuarioSesionId,
    string? Observaciones
);
