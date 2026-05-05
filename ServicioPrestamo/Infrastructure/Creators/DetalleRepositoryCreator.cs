using ServicioPrestamo.Domain.Ports;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Infrastructure.Creators;

public class DetalleRepositoryCreator
{
    private readonly IConfiguration _configuration;

    public DetalleRepositoryCreator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDetalleRepositorio CreateRepository()
    {
        return new DetalleRepository();
    }
}
