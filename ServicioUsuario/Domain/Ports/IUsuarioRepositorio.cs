using ServicioUsuario.Domain.Entities;

namespace ServicioUsuario.Domain.Ports;

public interface IUsuarioRepositorio
{
    Usuario? GetByNombreUsuario(string nombreUsuario);
    bool ExisteNombreUsuario(string nombreUsuario);
    bool ExisteEmail(string email);
    bool ExisteCi(string ci);
    Usuario? GetByCi(string ci);
    Usuario? GetById(int id);
    string JoinCiComp(string ci, string complemento);
}
