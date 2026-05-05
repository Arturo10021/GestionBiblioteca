using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Ports;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Infrastructure.Creators;

public class AutorRepositoryCreator : RepositoryFactory<Autor,int>
{
    private readonly IConfiguration _configuration;

    public AutorRepositoryCreator(IConfiguration configuration)
        : base(configuration.GetConnectionString("DefaultConnection")!)
    {
        _configuration = configuration;
    }

    public override IRepository<Autor,int> CreateRepository()
    {
        return new AutorRepository();
    }
}
