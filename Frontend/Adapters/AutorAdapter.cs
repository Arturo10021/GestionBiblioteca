using System.Net.Http.Json;
using Frontend.Dtos;
using Frontend.Adapters;
using Frontend.Helpers;

namespace Frontend.Adapters;

public class AutorAdapter : IAutorServicio
{
    private readonly HttpClient _http;

    public AutorAdapter(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ServicioPrestamo");
    }

    public IEnumerable<AutorDto> Select()
    {
        var response = _http.GetAsync("api/autores").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadFromJsonAsync<List<AutorDto>>().Result ?? new();
    }

    public AutorDto? GetById(int id)
    {
        var response = _http.GetAsync($"api/autores/{id}").Result;
        if (!response.IsSuccessStatusCode) return null;
        return response.Content.ReadFromJsonAsync<AutorDto>().Result;
    }

    public Result<AutorDto> Create(AutorDto dto)
    {
        try
        {
            var response = _http.PostAsJsonAsync("api/autores", dto).Result;
            if (!response.IsSuccessStatusCode)
                return Result<AutorDto>.Failure(new Error("Create", "Error al crear AutorDto"));
            var created = response.Content.ReadFromJsonAsync<AutorDto>().Result!;
            return Result<AutorDto>.Success(created);
        }
        catch (Exception ex)
        {
            return Result<AutorDto>.Failure(new Error("Create", ex.Message));
        }
    }

    public Result<AutorDto> Update(AutorDto dto)
    {
        try
        {
            var response = _http.PutAsJsonAsync($"api/autores/{dto.AutorId}", dto).Result;
            return response.IsSuccessStatusCode
                ? Result<AutorDto>.Success(dto)
                : Result<AutorDto>.Failure(new Error("Update", "Error al actualizar"));
        }
        catch (Exception ex)
        {
            return Result<AutorDto>.Failure(new Error("Update", ex.Message));
        }
    }

    public Result Delete(int id)
    {
        try
        {
            var response = _http.DeleteAsync($"api/autores/{id}").Result;
            return response.IsSuccessStatusCode
                ? Result.Success()
                : Result.Failure(new Error("Delete", "Error al eliminar"));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Delete", ex.Message));
        }
    }

    public Dictionary<int, string> ObtenerAutoresActivos() => new();
    public bool ExisteAutorActivo(int autorId) => true;
}
