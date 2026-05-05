using Microsoft.AspNetCore.Mvc;
using ServicioUsuario.Application.Dtos;
using ServicioUsuario.Application.Services;

namespace ServicioUsuario.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAllAsync()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioDto>> GetByIdAsync(int id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado" });

        return Ok(usuario);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<UsuarioDto>> GetByEmailAsync(string email)
    {
        var usuario = await _usuarioService.GetByEmailAsync(email);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado" });

        return Ok(usuario);
    }

    [HttpGet("ci/{ci}")]
    public async Task<ActionResult<UsuarioDto>> GetByCIAsync(string ci)
    {
        var usuario = await _usuarioService.GetByCIAsync(ci);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado" });

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> CreateAsync([FromBody] CreateUsuarioDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuario = await _usuarioService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = usuario.UsuarioId }, usuario);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UsuarioDto>> UpdateAsync(int id, [FromBody] UpdateUsuarioDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuario = await _usuarioService.UpdateAsync(id, dto);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado" });

        return Ok(usuario);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _usuarioService.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = "Usuario no encontrado" });

        return NoContent();
    }
}
