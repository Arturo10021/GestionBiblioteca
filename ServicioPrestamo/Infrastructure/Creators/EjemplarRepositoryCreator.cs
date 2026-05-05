using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Ports;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Infrastructure.Creators;

public class EjemplarRepositoryCreator : RepositoryFactory<Ejemplar,int>
{
    private readonly IConfiguration _configuration;

    public EjemplarRepositoryCreator(IConfiguration configuration)
        : base(configuration.GetConnectionString("DefaultConnection")!)
    {
        _configuration = configuration;
    }

    public override IRepository<Ejemplar,int> CreateRepository()
    {
        return new EjemplarRepository();
    }
}