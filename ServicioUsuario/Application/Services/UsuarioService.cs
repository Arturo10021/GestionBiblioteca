using ServicioUsuario.Application.Dtos;
using ServicioUsuario.Application.Interfaces;
using ServicioUsuario.Domain.Entities;
using ServicioUsuario.Domain.Ports;
using ServicioUsuario.Infrastructure.Persistence;
using System.Globalization;
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
    private readonly IUserCredentialProvisioningService _credentialProvisioning;

    public UsuarioService(UsuarioRepository repositorio, IUserCredentialProvisioningService credentialProvisioning)
    {
        _repositorio = repositorio;
        _credentialProvisioning = credentialProvisioning;
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

    public async Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombres)
            || string.IsNullOrWhiteSpace(dto.PrimerApellido)
            || string.IsNullOrWhiteSpace(dto.Email)
            || string.IsNullOrWhiteSpace(dto.Rol))
        {
            throw new InvalidOperationException("Completa todos los campos obligatorios.");
        }

        if (!string.IsNullOrWhiteSpace(dto.CI) && _repositorio.ExisteCi(dto.CI))
        {
            throw new InvalidOperationException("Ya existe un usuario registrado con ese CI.");
        }

        if (_repositorio.ExisteEmail(dto.Email))
        {
            throw new InvalidOperationException("Ya existe un usuario registrado con ese correo.");
        }

        var usuario = new Usuario
        {
            CI = dto.CI,
            Nombres = NormalizeDisplayName(dto.Nombres),
            PrimerApellido = NormalizeDisplayName(dto.PrimerApellido),
            SegundoApellido = NormalizeDisplayName(dto.SegundoApellido),
            Email = dto.Email,
            Rol = dto.Rol,
            Estado = true,
            FechaCreacion = DateTime.UtcNow
        };

        // Generate unique username, secure password, and send email
        var provisioningResult = await _credentialProvisioning.PrepareAndNotifyAsync(usuario);

        usuario.NombreUsuario = provisioningResult.GeneratedUserName;
        usuario.PasswordHash = provisioningResult.PasswordHash;

        _repositorio.Insert(usuario);

        var usuarioPersistido = usuario.UsuarioId > 0
            ? _repositorio.GetById(usuario.UsuarioId)
            : _repositorio.GetByNombreUsuario(usuario.NombreUsuario ?? string.Empty);

        if (usuarioPersistido == null)
        {
            throw new InvalidOperationException("No se pudo confirmar el registro del usuario.");
        }

        return MapToDto(usuarioPersistido);
    }

    public Task<UsuarioDto?> UpdateAsync(int id, UpdateUsuarioDto dto)
    {
        var usuario = _repositorio.GetById(id);
        if (usuario == null)
            return Task.FromResult<UsuarioDto?>(null);

        usuario.CI = dto.CI;
        usuario.Nombres = NormalizeDisplayName(dto.Nombres);
        usuario.PrimerApellido = NormalizeDisplayName(dto.PrimerApellido);
        usuario.SegundoApellido = NormalizeDisplayName(dto.SegundoApellido);
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
            Nombres = NormalizeDisplayName(usuario.Nombres),
            PrimerApellido = NormalizeDisplayName(usuario.PrimerApellido),
            SegundoApellido = NormalizeDisplayName(usuario.SegundoApellido),
            Email = usuario.Email,
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol,
            Estado = usuario.Estado
        };
    }

    private static string NormalizeDisplayName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var compactado = string.Join(' ', value
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        var textInfo = CultureInfo.GetCultureInfo("es-ES").TextInfo;
        return textInfo.ToTitleCase(textInfo.ToLower(compactado));
    }
}
