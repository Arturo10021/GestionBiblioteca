using System.Net.Http.Json;
using Frontend.Dtos;
using Frontend.Adapters;
using Frontend.Helpers;
using Frontend.Dtos;

namespace Frontend.Adapters;

public class LibroAdapter : ILibroServicio
{
    private readonly HttpClient _http;
    public LibroAdapter(IHttpClientFactory f) => _http = f.CreateClient("ServicioPrestamo");

    public IEnumerable<LibroDto> Select() => CallGet<List<LibroDto>>("api/libros");
    public LibroDto? GetById(int id) => CallGet<LibroDto>($"api/libros/{id}");
    public Result Create(LibroDto dto, string? n) => CallPost("api/libros", dto);
    public Result Update(LibroDto dto) => CallPut($"api/libros/{dto.LibroId}", dto);
    public Result Delete(int id, int? uid) => CallDelete($"api/libros/{id}");
    public Dictionary<int, string> ObtenerNombresAutores() => CallGet<Dictionary<int, string>>("api/libros/titulos") ?? new();
    public IEnumerable<AutorDto> ObtenerAutoresActivos() => CallGet<List<AutorDto>>("api/autores") ?? new();
    public bool ExisteAutorActivo(int id) => true;
    public int InsertarAutorYObtenerID(string n, int? uid) => 0;

    private T? CallGet<T>(string url) where T : class { try { var r = _http.GetAsync(url).Result; r.EnsureSuccessStatusCode(); return r.Content.ReadFromJsonAsync<T>().Result; } catch { return null; } }
    private Result CallPost(string url, object d) { try { var r = _http.PostAsJsonAsync(url, d).Result; return r.IsSuccessStatusCode ? Result.Success() : Result.Failure(new Error("Post", "Error")); } catch (Exception ex) { return Result.Failure(new Error("Post", ex.Message)); } }
    private Result CallPut(string url, object d) { try { var r = _http.PutAsJsonAsync(url, d).Result; return r.IsSuccessStatusCode ? Result.Success() : Result.Failure(new Error("Put", "Error")); } catch (Exception ex) { return Result.Failure(new Error("Put", ex.Message)); } }
    private Result CallDelete(string url) { try { var r = _http.DeleteAsync(url).Result; return r.IsSuccessStatusCode ? Result.Success() : Result.Failure(new Error("Delete", "Error")); } catch (Exception ex) { return Result.Failure(new Error("Delete", ex.Message)); } }
}
