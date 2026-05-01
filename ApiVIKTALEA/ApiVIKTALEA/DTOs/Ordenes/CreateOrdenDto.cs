using System.ComponentModel.DataAnnotations;

namespace ApiVIKTALEA.DTOs.Ordenes;

public class CreateOrdenDto
{
    [Required(ErrorMessage = "La orden debe contener al menos un ítem.")]
    public List<CreateDetalleOrdenDto> Items { get; set; } = new();
}

public class CreateDetalleOrdenDto
{
    [Required(ErrorMessage = "El producto es obligatorio.")]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }
}
