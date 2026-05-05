using Microsoft.AspNetCore.Mvc;
using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Domain.Entities;

namespace ServicioPrestamo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibrosController : ControllerBase
{
    private readonly ILibroServicio _libroServicio;

    public LibrosController(ILibroServicio libroServicio)
    {
        _libroServicio = libroServicio;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Libro>> GetAll()
    {
        return Ok(_libroServicio.Select());
    }

    [HttpGet("{id}")]
    public ActionResult<Libro> GetById(int id)
    {
        var libro = _libroServicio.GetById(id);
        return libro is null ? NotFound() : Ok(libro);
    }

    [HttpGet("titulos")]
    public ActionResult<Dictionary<int, string>> GetTitulos()
    {
        return Ok(_libroServicio.ObtenerNombresAutores());
    }

    [HttpPost]
    public ActionResult Create(Libro dto)
    {
        var result = _libroServicio.Create(dto, null);
        return result.IsFailure ? BadRequest(new { error = result.Error.Message }) : Ok();
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, Libro dto)
    {
        if (id != dto.LibroId)
            return BadRequest(new { error = "El ID no coincide" });
        var result = _libroServicio.Update(dto);
        return result.IsFailure ? BadRequest(new { error = result.Error.Message }) : NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var result = _libroServicio.Delete(id, null);
        return result.IsFailure ? BadRequest(new { error = result.Error.Message }) : NoContent();
    }
}
