using ServicioUsuario.Domain.Common;
using ServicioUsuario.Domain.Entities;
using ServicioUsuario.Domain.Ports;
using ServicioUsuario.Infrastructure.Persistence;

namespace ServicioUsuario.Infrastructure.Creators;

public class UsuarioRepositoryCreator
{
    private readonly IConfiguration _configuration;

    public UsuarioRepositoryCreator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IUsuarioRepositorio CreateRepository()
    {
        return new UsuarioRepository(_configuration);
    }
}
