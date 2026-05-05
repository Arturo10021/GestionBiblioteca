using ServicioPrestamo.Domain.Ports;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Infrastructure.Creators;

public class PrestamoRepositoryCreator
{
    private readonly IConfiguration _configuration;

    public PrestamoRepositoryCreator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IPrestamoRepositorio CreateRepository()
    {
        return new PrestamoRepository();
    }
}
