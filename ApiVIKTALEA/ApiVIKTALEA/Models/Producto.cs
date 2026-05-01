namespace ApiVIKTALEA.Models;

public class Producto : AuditableEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int CategoriaId { get; set; }
    public bool Estado { get; set; } = true;

    public Categoria Categoria { get; set; } = null!;
    public ICollection<DetalleOrden> DetallesOrden { get; set; } = new List<DetalleOrden>();
}
