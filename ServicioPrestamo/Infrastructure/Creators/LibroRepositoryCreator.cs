using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Ports;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Infrastructure.Creators;

public class LibroRepositoryCreator : RepositoryFactory<Libro, int>
{
    private readonly IConfiguration _configuration;

    public LibroRepositoryCreator(IConfiguration configuration)
        : base(configuration.GetConnectionString("DefaultConnection")!)
    {
        _configuration = configuration;
    }

    public override IRepository<Libro, int> CreateRepository()
    {
        return new LibroRepository();
    }
}