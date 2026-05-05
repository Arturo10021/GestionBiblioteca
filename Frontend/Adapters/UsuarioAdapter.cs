using System.Net.Http.Json;
using Frontend.Dtos;
using Frontend.Adapters;
using Frontend.Helpers;
using Frontend.Dtos;

namespace Frontend.Adapters;

public class UsuarioAdapter : IUsuarioServicio
{
    private readonly HttpClient _http;
    public UsuarioAdapter(IHttpClientFactory f) => _http = f.CreateClient("ServicioUsuario");

    public IEnumerable<UsuarioDto> Select() => new List<UsuarioDto>();
    public Result<UsuarioDto> Create(UsuarioDto d) => Result<UsuarioDto>.Failure(new Error("NotImpl", "Not implemented"));
    public Result CrearLector(LectorDto d, int uid) => Result.Failure(new Error("NotImpl", "Not implemented"));
    public Result DarDeBaja(int uid, int sid) => Result.Failure(new Error("NotImpl", "Not implemented"));
    public async Task<Result> CrearUsuarioAsync(UsuarioDto d, int uid, CancellationToken ct = default) => Result.Failure(new Error("NotImpl", "Not implemented"));
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
}
