using ApiVIKTALEA.DTOs.Common;
using ApiVIKTALEA.DTOs.Productos;

namespace ApiVIKTALEA.Services;

public interface IProductoService
{
    Task<PagedResultDto<ProductoDto>> GetAllAsync(ProductoFilterDto filter);
    Task<ProductoDto?> GetByIdAsync(int id);
    Task<(ProductoDto? producto, string? error)> CreateAsync(CreateProductoDto dto);
    Task<(ProductoDto? producto, string? error)> UpdateAsync(int id, UpdateProductoDto dto);
    Task<(bool success, string? error)> DeactivateAsync(int id);
}
