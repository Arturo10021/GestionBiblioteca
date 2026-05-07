using Microsoft.AspNetCore.Mvc;
using ServicioPrestamo.Application.Interfaces;

namespace ServicioPrestamo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallesController : ControllerBase
{
    private readonly IDetalleServicio _detalleServicio;

    public DetallesController(IDetalleServicio detalleServicio)
    {
        _detalleServicio = detalleServicio;
    }

    [HttpGet]
    public ActionResult GetAll()
    {
        var detalles = _detalleServicio.ObtenerTodos();
        return Ok(detalles);
    }

    [HttpGet("prestamo/{prestamoId}")]
    public ActionResult GetByPrestamoId(int prestamoId)
    {
        var detalles = _detalleServicio.ObtenerPorPrestamo(prestamoId);
        return Ok(detalles);
    }
}
