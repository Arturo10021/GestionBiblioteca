namespace ServicioUsuario.Application.Dtos;

public class UsuarioDto
{
    public int UsuarioId { get; set; }
    public string CI { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? NombreUsuario { get; set; }
    public string Rol { get; set; } = string.Empty;
    public bool Estado { get; set; }
}

public class CreateUsuarioDto
{
    public string CI { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? NombreUsuario { get; set; }
    public string? Password { get; set; }
    public string Rol { get; set; } = string.Empty;
}

public class UpdateUsuarioDto
{
    public string CI { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? NombreUsuario { get; set; }
    public string Rol { get; set; } = string.Empty;
    public bool Estado { get; set; }
}
