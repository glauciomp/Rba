using SQLite;

namespace Rba.Models;

public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    [MaxLength(50)]
    public string NomeUsuario { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Senha { get; set; } = string.Empty;

    public string TipoUsuario { get; set; } = "Normal"; // Normal por padrão
}
