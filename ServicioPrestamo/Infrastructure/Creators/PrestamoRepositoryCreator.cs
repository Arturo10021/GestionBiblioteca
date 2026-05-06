using Microsoft.Extensions.Configuration;
using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Ports;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Infrastructure.Creators;

public class PrestamoRepositoryCreator : RepositoryFactory<Prestamo, int>
{
    private readonly IConfiguration _configuration;

    public PrestamoRepositoryCreator(IConfiguration configuration)
        : base(configuration.GetConnectionString("DefaultConnection")!)
    {
        _configuration = configuration;
    }

    public override IRepository<Prestamo, int> CreateRepository()
    {
        return new PrestamoRepository();
    }
}
