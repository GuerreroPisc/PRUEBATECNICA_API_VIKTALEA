namespace ApiVIKTALEA.Models;

public class User : AuditableEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Operador";
    public bool IsActive { get; set; } = true;
}
