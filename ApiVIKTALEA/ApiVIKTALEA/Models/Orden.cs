namespace ApiVIKTALEA.Models;

public class Orden : AuditableEntity
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string Estado { get; set; } = "Pendiente";
    public decimal Total { get; set; }

    public ICollection<DetalleOrden> Detalles { get; set; } = new List<DetalleOrden>();
}
