namespace ApiVIKTALEA.Models;

public abstract class AuditableEntity
{
    public string UsuarioCreacion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public string? UsuarioModificacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public string? UsuarioEliminacion { get; set; }
    public DateTime? FechaEliminacion { get; set; }
}
