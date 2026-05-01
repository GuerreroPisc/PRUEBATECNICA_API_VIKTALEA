using ApiVIKTALEA.DTOs.Common;
using ApiVIKTALEA.DTOs.Ordenes;

namespace ApiVIKTALEA.Services;

public interface IOrdenService
{
    Task<PagedResultDto<OrdenResumenDto>> GetAllAsync(OrdenFilterDto filter);
    Task<OrdenDto?> GetByIdAsync(int id);
    Task<(OrdenDto? orden, string? error)> CreateAsync(CreateOrdenDto dto);
}
