using System.Net.Http.Json;
using Frontend.Dtos;
using Frontend.Adapters;
using Frontend.Helpers;
using Frontend.Dtos;

namespace Frontend.Adapters;

public class EjemplarAdapter : IEjemplarServicio
{
    private readonly HttpClient _http;
    public EjemplarAdapter(IHttpClientFactory f) => _http = f.CreateClient("ServicioPrestamo");

    public IEnumerable<EjemplarDto> Select() => CallGet<List<EjemplarDto>>("api/ejemplares") ?? new();
    public EjemplarDto? GetById(int id) => CallGet<EjemplarDto>($"api/ejemplares/{id}");
    public Result<EjemplarDto> Create(EjemplarDto d) => CallPostR<EjemplarDto>("api/ejemplares", d);
    public Result<EjemplarDto> Update(EjemplarDto d) => CallPutR<EjemplarDto>($"api/ejemplares/{d.EjemplarId}", d);
    public Result Delete(EjemplarDto d) => CallDeleteR($"api/ejemplares/{d.EjemplarId}");
    public Dictionary<int, string> ObtenerTitulosLibros() => CallGet<Dictionary<int, string>>("api/libros/titulos") ?? new();
    public IEnumerable<LibroDto> ObtenerLibrosActivos() => CallGet<List<LibroDto>>("api/libros") ?? new();
    public bool ExisteLibroActivo(int id) => true;
    public Dictionary<int, string> ObtenerEjemplaresDisponibles() => new();
    public Result ValidarEjemplar(EjemplarDto e) => Result.Success();

    private T? CallGet<T>(string u) where T : class { try { var r = _http.GetAsync(u).Result; r.EnsureSuccessStatusCode(); return r.Content.ReadFromJsonAsync<T>().Result; } catch { return null; } }
    private Result<EjemplarDto> CallPostR<T>(string u, object d) { try { var r = _http.PostAsJsonAsync(u, d).Result; if (!r.IsSuccessStatusCode) return Result<EjemplarDto>.Failure(new Error("Post", "Error")); return Result<EjemplarDto>.Success(r.Content.ReadFromJsonAsync<EjemplarDto>().Result!); } catch (Exception ex) { return Result<EjemplarDto>.Failure(new Error("Post", ex.Message)); } }
    private Result<EjemplarDto> CallPutR<T>(string u, object d) { try { var r = _http.PutAsJsonAsync(u, d).Result; return r.IsSuccessStatusCode ? Result<EjemplarDto>.Success(d as EjemplarDto ?? new()) : Result<EjemplarDto>.Failure(new Error("Put", "Error")); } catch (Exception ex) { return Result<EjemplarDto>.Failure(new Error("Put", ex.Message)); } }
    private Result CallDeleteR(string u) { try { var r = _http.DeleteAsync(u).Result; return r.IsSuccessStatusCode ? Result.Success() : Result.Failure(new Error("Delete", "Error")); } catch (Exception ex) { return Result.Failure(new Error("Delete", ex.Message)); } }
}
