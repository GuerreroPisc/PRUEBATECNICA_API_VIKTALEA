namespace ApiVIKTALEA.DTOs.Productos;

public class ProductoFilterDto
{
    public string? Nombre { get; set; }
    public int? CategoriaId { get; set; }
    public bool? Estado { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
