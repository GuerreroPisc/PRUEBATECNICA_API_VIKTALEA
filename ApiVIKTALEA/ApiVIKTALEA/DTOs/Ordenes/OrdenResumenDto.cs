namespace ApiVIKTALEA.DTOs.Ordenes;

public class OrdenResumenDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int CantidadItems { get; set; }
}
