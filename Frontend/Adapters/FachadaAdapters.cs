using Frontend.Dtos;
using Frontend.Adapters;
using Frontend.Adapters;
using Frontend.Helpers;
using Frontend.Dtos;

namespace Frontend.Adapters;

public class PrestamoFachadaAdapter : IPrestamoFachada
{
    public IEnumerable<KeyValuePair<int, string>> BuscarEjemplaresActivos(string q) => new List<KeyValuePair<int, string>>();
    public IEnumerable<KeyValuePair<int, string>> BuscarLectoresPorCi(string q) => new List<KeyValuePair<int, string>>();
    public Result<int> CrearPrestamoMultiple(int lectorId, IEnumerable<int> ejIds, DateTime f, int? uid = null, string? obs = null) => Result<int>.Failure(new Error("NotImpl", "Not implemented"));
    public Result<int> CrearPrestamoMultiple(int lectorId, IEnumerable<(int, string?)> d, DateTime f, int? uid = null) => Result<int>.Failure(new Error("NotImpl", "Not implemented"));
    public Result CrearPrestamo(PrestamoDto p) => Result.Failure(new Error("NotImpl", "Not implemented"));
    public Result CrearPrestamos(IEnumerable<PrestamoDto> p) => Result.Failure(new Error("NotImpl", "Not implemented"));
    public int CountPrestamosActivos(int id) => 0;
    public PrestamoDto? ObtenerPrestamoPorId(int id) => null;
    public EjemplarDto? ObtenerEjemplarPorId(int id) => null;
    public string? ObtenerLabelEjemplar(int id) => null;
    public UsuarioDto? ObtenerUsuarioPorCi(string ci) => null;
    public List<object> ObtenerTodosLosLectores() => new();
}

public class AnulacionFachadaAdapter : IAnulacionFachada
{
    public Result AnularPrestamo(int id, int? uid, string m) => Result.Success();
}

public class EjemplarDisponibilidadFachadaAdapter : IEjemplarDisponibilidadFachada
{
    public Result CambiarDisponibilidad(int id, bool d, int? uid) => Result.Success();
}

public class PrestamoServicioAdapter : IPrestamoServicio
{
    public IEnumerable<PrestamoDto> Select() => new List<PrestamoDto>();
    public Result<PrestamoDto> Create(PrestamoDto d) => Result<PrestamoDto>.Failure(new Error("NotImpl", "Not implemented"));
    public Result<PrestamoDto> Update(PrestamoDto d) => Result<PrestamoDto>.Failure(new Error("NotImpl", "Not implemented"));
    public Result Delete(PrestamoDto d) => Result.Failure(new Error("NotImpl", "Not implemented"));
    public PrestamoDto? GetById(int id) => null;
    public Result ValidarPrestamo(PrestamoDto p) => Result.Success();
    public int CountPrestamosActivos(int id) => 0;
    public int InsertAndReturnId(PrestamoDto p) => 0;
}

public class DetalleServicioAdapter : IDetalleServicio
{
    public IEnumerable<DetalleDto> Select() => new List<DetalleDto>();
    public IEnumerable<DetalleDto> ObtenerPorPrestamo(int id) => new List<DetalleDto>();
    public IEnumerable<DetalleDto> ObtenerTodos() => new List<DetalleDto>();
    public Result CrearMultiples(IEnumerable<DetalleDto> d) => Result.Success();
}
