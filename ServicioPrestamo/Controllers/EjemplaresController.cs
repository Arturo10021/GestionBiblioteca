using Microsoft.AspNetCore.Mvc;
using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EjemplaresController : ControllerBase
{
    private readonly IEjemplarServicio _ejemplarServicio;

    public EjemplaresController(IEjemplarServicio ejemplarServicio)
    {
        _ejemplarServicio = ejemplarServicio;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Ejemplar>> GetAll()
    {
        return Ok(_ejemplarServicio.Select());
    }

    [HttpGet("{id}")]
    public ActionResult<Ejemplar> GetById(int id)
    {
        var ejemplar = _ejemplarServicio.GetById(id);
        return ejemplar is null ? NotFound() : Ok(ejemplar);
    }

    [HttpPost]
    public ActionResult<Ejemplar> Create(Ejemplar dto)
    {
        var result = _ejemplarServicio.Create(dto);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });
        return CreatedAtAction(nameof(GetById), new { id = result.Value.EjemplarId }, result.Value);
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, Ejemplar dto)
    {
        if (id != dto.EjemplarId)
            return BadRequest(new { error = "El ID no coincide" });
        var result = _ejemplarServicio.Update(dto);
        return result.IsFailure ? BadRequest(new { error = result.Error.Message }) : NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var ejemplar = _ejemplarServicio.GetById(id);
        if (ejemplar is null) return NotFound();
        var result = _ejemplarServicio.Delete(ejemplar);
        return result.IsFailure ? BadRequest(new { error = result.Error.Message }) : NoContent();
    }
}
