using ServicioUsuario.Application.Dtos;
using ServicioUsuario.Domain.Entities;
using ServicioUsuario.Domain.Ports;
using ServicioUsuario.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;

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
    Task<UsuarioDto?> LoginAsync(string nombreUsuario, string password);
}

public class UsuarioService : IUsuarioService
{
    private readonly UsuarioRepository _repositorio;

    public UsuarioService(UsuarioRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<List<UsuarioDto>> GetAllAsync()
    {
        var usuarios = _repositorio.GetAll()
            .Where(u => u.Estado)
            .Select(MapToDto)
            .ToList();
        return Task.FromResult(usuarios);
    }

    public Task<UsuarioDto?> GetByIdAsync(int id)
    {
        var usuario = _repositorio.GetById(id);
        return Task.FromResult(usuario != null ? MapToDto(usuario) : null);
    }

    public Task<UsuarioDto?> GetByEmailAsync(string email)
    {
        var usuarios = _repositorio.GetAll();
        var usuario = usuarios.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(usuario != null ? MapToDto(usuario) : null);
    }

    public Task<UsuarioDto?> GetByCIAsync(string ci)
    {
        var usuario = _repositorio.GetByCi(ci);
        return Task.FromResult(usuario != null ? MapToDto(usuario) : null);
    }

    public Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto)
    {
        var usuario = new Usuario
        {
            CI = dto.CI,
            Nombres = dto.Nombres,
            PrimerApellido = dto.PrimerApellido,
            SegundoApellido = dto.SegundoApellido,
            Email = dto.Email,
            NombreUsuario = !string.IsNullOrWhiteSpace(dto.NombreUsuario)
                ? dto.NombreUsuario
                : (!string.IsNullOrWhiteSpace(dto.CI) ? dto.CI : dto.Email),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password ?? "temporal123"),
            Rol = dto.Rol,
            Estado = true,
            FechaCreacion = DateTime.UtcNow
        };

        _repositorio.Insert(usuario);

        var usuarioPersistido = !string.IsNullOrWhiteSpace(usuario.CI)
            ? _repositorio.GetByCi(usuario.CI)
            : _repositorio.GetByNombreUsuario(usuario.NombreUsuario ?? string.Empty);

        return Task.FromResult(MapToDto(usuarioPersistido ?? usuario));
    }

    public Task<UsuarioDto?> UpdateAsync(int id, UpdateUsuarioDto dto)
    {
        var usuario = _repositorio.GetById(id);
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

        // TODO: Implementar actualización en repositorio
        return Task.FromResult<UsuarioDto?>(MapToDto(usuario));
    }

    public Task<bool> DeleteAsync(int id)
    {
        var usuario = _repositorio.GetById(id);
        if (usuario == null)
            return Task.FromResult(false);

        usuario.Estado = false;
        // TODO: Implementar eliminación lógica en repositorio
        return Task.FromResult(true);
    }

    public Task<UsuarioDto?> LoginAsync(string nombreUsuario, string password)
    {
        var usuario = _repositorio.GetByNombreUsuario(nombreUsuario);

        if (usuario == null || !usuario.Estado || usuario.PasswordHash == null)
            return Task.FromResult<UsuarioDto?>(null);

        if (!VerifyPassword(password, usuario.PasswordHash))
            return Task.FromResult<UsuarioDto?>(null);

        return Task.FromResult<UsuarioDto?>(MapToDto(usuario));
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                return true;
        }
        catch
        {
            // Hash legado o formato no compatible con BCrypt.
        }

        return string.Equals(ComputeSha256(password), storedHash, StringComparison.Ordinal);
    }

    private static string ComputeSha256(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
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
