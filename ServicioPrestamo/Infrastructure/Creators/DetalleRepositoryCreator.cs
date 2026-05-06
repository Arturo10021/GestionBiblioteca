using Microsoft.Extensions.Configuration;
using ServicioPrestamo.Domain.Entities;
using ServicioPrestamo.Domain.Ports;
using ServicioPrestamo.Infrastructure.Persistence;

namespace ServicioPrestamo.Infrastructure.Creators;

public class DetalleRepositoryCreator : RepositoryFactory<Detalle, int>
{
    private readonly IConfiguration _configuration;

    public DetalleRepositoryCreator(IConfiguration configuration)
        : base(configuration.GetConnectionString("DefaultConnection")!)
    {
        _configuration = configuration;
    }

    public override IRepository<Detalle, int> CreateRepository()
    {
        return new DetalleRepository();
    }
}
