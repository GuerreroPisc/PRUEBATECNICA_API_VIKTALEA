using ApiVIKTALEA.DTOs.Categorias;

namespace ApiVIKTALEA.Services;

public interface ICategoriaService
{
    Task<List<CategoriaDto>> GetAllAsync();
    Task<CategoriaDto?> GetByIdAsync(int id);
    Task<CategoriaDto> CreateAsync(CreateCategoriaDto dto);
    Task<CategoriaDto?> UpdateAsync(int id, UpdateCategoriaDto dto);
    Task<(bool success, string? error)> DeactivateAsync(int id);
}
