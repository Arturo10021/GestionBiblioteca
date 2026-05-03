using MySql.Data.MySqlClient;

namespace ServicioUsuario.Infrastructure.Configuration;

public class ConfigurationSingleton
{
    private static readonly ConfigurationSingleton _instancia = new();
    private readonly string _connectionString = "Server=localhost;Database=gestion_biblioteca;User Id=root;Password=;";

    public static ConfigurationSingleton Instancia => _instancia;

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
