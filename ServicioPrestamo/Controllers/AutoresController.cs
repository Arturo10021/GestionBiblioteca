using Microsoft.AspNetCore.Mvc;
using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutoresController : ControllerBase
{
    private readonly IAutorServicio _autorServicio;

    public AutoresController(IAutorServicio autorServicio)
    {
        _autorServicio = autorServicio;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Autor>> GetAll()
    {
        return Ok(_autorServicio.Select());
    }

    [HttpGet("{id}")]
    public ActionResult<Autor> GetById(int id)
    {
        var autor = _autorServicio.GetById(id);
        return autor is null ? NotFound() : Ok(autor);
    }

    [HttpPost]
    public ActionResult<Autor> Create(Autor dto)
    {
        var result = _autorServicio.Create(dto);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });
        return CreatedAtAction(nameof(GetById), new { id = result.Value.AutorId }, result.Value);
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, Autor dto)
    {
        if (id != dto.AutorId)
            return BadRequest(new { error = "El ID no coincide" });
        var result = _autorServicio.Update(dto);
        return result.IsFailure ? BadRequest(new { error = result.Error.Message }) : NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var result = _autorServicio.Delete(id);
        return result.IsFailure ? BadRequest(new { error = result.Error.Message }) : NoContent();
    }
}
