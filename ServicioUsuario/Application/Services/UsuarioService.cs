using ServicioUsuario.Application.Dtos;
using ServicioUsuario.Domain.Entities;

namespace ServicioUsuario.Application.Services;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> GetAllAsync();
    Task<UsuarioDto?> GetByIdAsync(int id);
    Task<UsuarioDto?> GetByEmailAsync(string email);
    Task<UsuarioDto?> GetByCIAsync(string ci);
    Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto);
    Task<UsuarioDto?> UpdateAsync(int id, UpdateUsuarioDto dto);
    Task<bool> DeleteAsync(int id);
}

public class UsuarioService : IUsuarioService
{
    private static List<Usuario> _usuarios = new();

    public UsuarioService()
    {
        _usuarios = new List<Usuario>
        {
            new Usuario
            {
                UsuarioId = 1,
                CI = "12345678",
                Nombres = "Juan",
                PrimerApellido = "Pérez",
                SegundoApellido = "García",
                Email = "juan.perez@example.com",
                NombreUsuario = "jperez",
                Rol = "Administrador",
                Estado = true
            },
            new Usuario
            {
                UsuarioId = 2,
                CI = "87654321",
                Nombres = "María",
                PrimerApellido = "López",
                SegundoApellido = "Martínez",
                Email = "maria.lopez@example.com",
                NombreUsuario = "mlopez",
                Rol = "Bibliotecario",
                Estado = true
            }
        };
    }

    public Task<List<UsuarioDto>> GetAllAsync()
    {
        var usuarios = _usuarios
            .Where(u => u.Estado)
            .Select(MapToDto)
            .ToList();
        return Task.FromResult(usuarios);
    }

    public Task<UsuarioDto?> GetByIdAsync(int id)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.UsuarioId == id);
        return Task.FromResult(usuario != null ? MapToDto(usuario) : null);
    }

    public Task<UsuarioDto?> GetByEmailAsync(string email)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(usuario != null ? MapToDto(usuario) : null);
    }

    public Task<UsuarioDto?> GetByCIAsync(string ci)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.CI == ci);
        return Task.FromResult(usuario != null ? MapToDto(usuario) : null);
    }

    public Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto)
    {
        var usuario = new Usuario
        {
            UsuarioId = _usuarios.Any() ? _usuarios.Max(u => u.UsuarioId) + 1 : 1,
            CI = dto.CI,
            Nombres = dto.Nombres,
            PrimerApellido = dto.PrimerApellido,
            SegundoApellido = dto.SegundoApellido,
            Email = dto.Email,
            NombreUsuario = dto.NombreUsuario,
            PasswordHash = !string.IsNullOrEmpty(dto.Password) ? BCrypt.Net.BCrypt.HashPassword(dto.Password) : null,
            Rol = dto.Rol,
            Estado = true,
            FechaCreacion = DateTime.UtcNow
        };

        _usuarios.Add(usuario);
        return Task.FromResult(MapToDto(usuario));
    }

    public Task<UsuarioDto?> UpdateAsync(int id, UpdateUsuarioDto dto)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.UsuarioId == id);
        if (usuario == null)
            return Task.FromResult<UsuarioDto?>(null);

        usuario.CI = dto.CI;
        usuario.Nombres = dto.Nombres;
        usuario.PrimerApellido = dto.PrimerApellido;
        usuario.SegundoApellido = dto.SegundoApellido;
        usuario.Email = dto.Email;
        usuario.NombreUsuario = dto.NombreUsuario;
        usuario.Rol = dto.Rol;
        usuario.Estado = dto.Estado;
        usuario.FechaActualizacion = DateTime.UtcNow;

        return Task.FromResult<UsuarioDto?>(MapToDto(usuario));
    }

    public Task<bool> DeleteAsync(int id)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.UsuarioId == id);
        if (usuario == null)
            return Task.FromResult(false);

        usuario.Estado = false;
        return Task.FromResult(true);
    }

    private UsuarioDto MapToDto(Usuario usuario)
    {
        return new UsuarioDto
        {
            UsuarioId = usuario.UsuarioId,
            CI = usuario.CI,
            Nombres = usuario.Nombres,
            PrimerApellido = usuario.PrimerApellido,
            SegundoApellido = usuario.SegundoApellido,
            Email = usuario.Email,
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol,
            Estado = usuario.Estado
        };
    }
}
