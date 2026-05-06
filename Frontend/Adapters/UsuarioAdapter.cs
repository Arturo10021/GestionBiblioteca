using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Dtos;
using Frontend.Adapters;
using Frontend.Helpers;
using Frontend.Dtos;

namespace Frontend.Adapters;

public class UsuarioAdapter : IUsuarioServicio
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public UsuarioAdapter(IHttpClientFactory f) => _http = f.CreateClient("ServicioUsuario");

    public IEnumerable<UsuarioDto> Select()
    {
        try
        {
            var response = _http.GetAsync("api/usuarios").Result;
            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"Error UsuarioAdapter.Select(): {response.StatusCode} - {response.Content.ReadAsStringAsync().Result}");
                return new List<UsuarioDto>();
            }

            var resultado = response.Content.ReadFromJsonAsync<List<UsuarioDto>>(JsonOptions).Result ?? new();
            System.Diagnostics.Debug.WriteLine($"UsuarioAdapter.Select() retornó {resultado.Count} usuarios");
            return resultado;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error UsuarioAdapter.Select() Exception: {ex.Message}");
            return new List<UsuarioDto>();
        }
    }

    public Result<UsuarioDto> Create(UsuarioDto d)
    {
        try
        {
            d.Nombres = d.Nombres.ToDisplayName();
            d.PrimerApellido = d.PrimerApellido.ToDisplayName();
            d.SegundoApellido = d.SegundoApellido.ToDisplayName();
            var response = _http.PostAsJsonAsync("api/usuarios", d).Result;
            if (!response.IsSuccessStatusCode)
                return Result<UsuarioDto>.Failure(new Error("Create", "Error al crear usuario"));

            var created = response.Content.ReadFromJsonAsync<UsuarioDto>().Result;
            return Result<UsuarioDto>.Success(created ?? d);
        }
        catch (Exception ex)
        {
            return Result<UsuarioDto>.Failure(new Error("Create", ex.Message));
        }
    }

    public Result CrearLector(LectorDto d, int uid)
    {
        try
        {
            var ciCompleto = JoinCiComp(d.CI ?? string.Empty, d.Complemento ?? string.Empty);
            var createUsuarioDto = new UsuarioDto
            {
                NombreUsuario = !string.IsNullOrWhiteSpace(ciCompleto)
                    ? ciCompleto
                    : d.Nombres,
                Nombres = d.Nombres.ToDisplayName(),
                PrimerApellido = d.PrimerApellido.ToDisplayName(),
                SegundoApellido = d.SegundoApellido.ToDisplayName(),
                Email = d.Email,
                CI = ciCompleto,
                Rol = "Lector",
                Estado = true
            };

            var response = _http.PostAsJsonAsync("api/usuarios", createUsuarioDto).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                System.Diagnostics.Debug.WriteLine($"Error CrearLector: {response.StatusCode} - {errorContent}");
                return Result.Failure(new Error("Create", $"Error al crear lector: {errorContent}"));
            }
            
            System.Diagnostics.Debug.WriteLine("Lector creado exitosamente");
            return Result.Success();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception CrearLector: {ex.Message}");
            return Result.Failure(new Error("Create", ex.Message));
        }
    }

    public Result DarDeBaja(int uid, int sid)
    {
        try
        {
            var usuario = CallGet<UsuarioDto>($"api/usuarios/{uid}");
            if (usuario == null) return Result.Failure(new Error("NotFound", "Usuario no encontrado"));

            usuario.Estado = false;
            var response = _http.PutAsJsonAsync($"api/usuarios/{uid}", usuario).Result;
            return response.IsSuccessStatusCode ? Result.Success() : Result.Failure(new Error("Update", "Error al dar de baja"));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Delete", ex.Message));
        }
    }

    public async Task<Result> CrearUsuarioAsync(UsuarioDto d, int uid, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/usuarios", d, ct);
            return response.IsSuccessStatusCode ? Result.Success() : Result.Failure(new Error("Create", "Error al crear usuario"));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Create", ex.Message));
        }
    }

    public string JoinCiComp(string ci, string comp) => string.IsNullOrWhiteSpace(comp) ? ci : $"{ci}-{comp}";

    public Result<UsuarioDto> Login(string user, string pass)
    {
        try
        {
            var response = _http.PostAsJsonAsync("api/usuarios/login", new { nombreUsuario = user, password = pass }).Result;
            if (!response.IsSuccessStatusCode)
                return Result<UsuarioDto>.Failure(new Error("Login", "Credenciales inválidas"));

            var dto = response.Content.ReadFromJsonAsync<UsuarioDto>().Result;
            if (dto == null)
                return Result<UsuarioDto>.Failure(new Error("Login", "Error al leer respuesta"));

            return Result<UsuarioDto>.Success(new UsuarioDto
            {
                UsuarioId = dto.UsuarioId,
                NombreUsuario = dto.NombreUsuario ?? user,
                Nombres = dto.Nombres,
                PrimerApellido = dto.PrimerApellido,
                Rol = dto.Rol,
                Estado = dto.Estado
            });
        }
        catch (Exception ex)
        {
            return Result<UsuarioDto>.Failure(new Error("Login", ex.Message));
        }
    }

    private T? CallGet<T>(string url) where T : class
    {
        try
        {
            var response = _http.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadFromJsonAsync<T>().Result;
        }
        catch
        {
            return null;
        }
    }
}
