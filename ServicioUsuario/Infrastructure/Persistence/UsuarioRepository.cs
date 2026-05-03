using System.Data;
using ServicioUsuario.Domain.Entities;
using ServicioUsuario.Domain.Ports;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using ServicioUsuario.Infrastructure.Configuration;
using System.Collections.Generic;

namespace ServicioUsuario.Infrastructure.Persistence;

public class UsuarioRepository : IUsuarioRepositorio
{
    private readonly IConfiguration? _configuration;

    public UsuarioRepository()
    {
    }

    public UsuarioRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Usuario? GetByCi(string ci)
    {
        Usuario? usuario = null;
        try
        {
            using (var connection = (MySqlConnection)ConfigurationSingleton.Instancia.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM usuario WHERE CI = @CI LIMIT 1;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CI", ci);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapReaderToUsuario(reader);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener usuario por CI: {ex.Message}");
        }
        return usuario;
    }

    public Usuario? GetById(int id)
    {
        Usuario? usuario = null;
        try
        {
            using (var connection = (MySqlConnection)ConfigurationSingleton.Instancia.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM usuario WHERE UsuarioId = @UsuarioId;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioId", id);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapReaderToUsuario(reader);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener usuario por ID: {ex.Message}");
        }
        return usuario;
    }

    public Usuario? GetByNombreUsuario(string nombreUsuario)
    {
        Usuario? usuario = null;
        try
        {
            using (var connection = (MySqlConnection)ConfigurationSingleton.Instancia.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM usuario WHERE NombreUsuario = @NombreUsuario LIMIT 1;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapReaderToUsuario(reader);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener usuario por nombre: {ex.Message}");
        }
        return usuario;
    }

    public bool ExisteNombreUsuario(string nombreUsuario)
    {
        try
        {
            using (var connection = (MySqlConnection)ConfigurationSingleton.Instancia.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(1) FROM usuario WHERE NombreUsuario = @NombreUsuario;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al verificar nombre de usuario: {ex.Message}");
            return false;
        }
    }

    public bool ExisteEmail(string email)
    {
        try
        {
            using (var connection = (MySqlConnection)ConfigurationSingleton.Instancia.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(1) FROM usuario WHERE Email = @Email;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al verificar email: {ex.Message}");
            return false;
        }
    }

    public bool ExisteCi(string ci)
    {
        try
        {
            using (var connection = (MySqlConnection)ConfigurationSingleton.Instancia.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(1) FROM usuario WHERE CI = @CI;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CI", ci);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al verificar CI: {ex.Message}");
            return false;
        }
    }

    public string JoinCiComp(string ci, string complemento)
    {
        if (string.IsNullOrWhiteSpace(complemento))
            return ci;
        return $"{ci}-{complemento}";
    }

    private Usuario MapReaderToUsuario(MySqlDataReader reader)
    {
        return new Usuario
        {
            UsuarioId = reader.GetInt32("UsuarioId"),
            Nombres = reader.GetString("Nombres"),
            PrimerApellido = reader.GetString("PrimerApellido"),
            SegundoApellido = reader.IsDBNull("SegundoApellido") ? null : reader.GetString("SegundoApellido"),
            Email = reader.GetString("Email"),
            NombreUsuario = reader.IsDBNull("NombreUsuario") ? null : reader.GetString("NombreUsuario"),
            PasswordHash = reader.IsDBNull("PasswordHash") ? null : reader.GetString("PasswordHash"),
            Rol = reader.GetString("Rol"),
            Estado = reader.GetBoolean("Estado"),
            CI = reader.IsDBNull("CI") ? null : reader.GetString("CI")
        };
    }
}
