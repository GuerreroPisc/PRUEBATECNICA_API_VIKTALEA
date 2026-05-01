using ApiVIKTALEA.Data;
using ApiVIKTALEA.DTOs.Common;
using ApiVIKTALEA.DTOs.Ordenes;
using ApiVIKTALEA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiVIKTALEA.Services;

public class OrdenService : IOrdenService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IProductoService _productoService;

    public OrdenService(AppDbContext context, IHttpContextAccessor httpContext, IProductoService productoService)
    {
        _context = context;
        _httpContext = httpContext;
        _productoService = productoService;
    }

    private string UsuarioActual =>
        _httpContext.HttpContext?.User?.Identity?.Name ?? "system";

    public async Task<PagedResultDto<OrdenResumenDto>> GetAllAsync(OrdenFilterDto filter)
    {
        var query = _context.Ordenes.Include(o => o.Detalles).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Estado))
            query = query.Where(o => o.Estado == filter.Estado);
        if (filter.FechaDesde.HasValue)
            query = query.Where(o => o.Fecha >= filter.FechaDesde);
        if (filter.FechaHasta.HasValue)
            query = query.Where(o => o.Fecha <= filter.FechaHasta);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(o => o.Fecha)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(o => new OrdenResumenDto
            {
                Id = o.Id,
                Fecha = o.Fecha,
                Estado = o.Estado,
                Total = o.Total,
                CantidadItems = o.Detalles.Count
            })
            .ToListAsync();

        return new PagedResultDto<OrdenResumenDto>
        {
            Items = items,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<OrdenDto?> GetByIdAsync(int id)
    {
        var orden = await _context.Ordenes
            .Include(o => o.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (orden is null) return null;
        return MapToDto(orden);
    }

    public async Task<(OrdenDto? orden, string? error)> CreateAsync(CreateOrdenDto dto)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            return (null, "La orden debe tener al menos un ítem.");

        var usuario = UsuarioActual;
        var ahora = DateTime.UtcNow;

        var orden = new Orden
        {
            Fecha = ahora,
            Estado = "Pendiente",
            UsuarioCreacion = usuario,
            FechaCreacion = ahora,
            Detalles = new List<DetalleOrden>()
        };

        foreach (var item in dto.Items)
        {
            var (success, error, precio, _) = await _productoService.ValidarYDescontarStockAsync(item.ProductoId, item.Cantidad);
            if (!success)
                return (null, error);

            orden.Detalles.Add(new DetalleOrden
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = precio,
                Subtotal = precio * item.Cantidad,
                UsuarioCreacion = usuario,
                FechaCreacion = ahora
            });
        }

        orden.Total = orden.Detalles.Sum(d => d.Subtotal);
        _context.Ordenes.Add(orden);
        await _context.SaveChangesAsync();

        await _context.Entry(orden).Collection(o => o.Detalles)
            .Query().Include(d => d.Producto).LoadAsync();

        return (MapToDto(orden), null);
    }

    private static OrdenDto MapToDto(Orden orden) => new()
    {
        Id = orden.Id,
        Fecha = orden.Fecha,
        Estado = orden.Estado,
        Total = orden.Total,
        Detalles = orden.Detalles.Select(d => new DetalleOrdenDto
        {
            Id = d.Id,
            ProductoId = d.ProductoId,
            ProductoNombre = d.Producto.Nombre,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario,
            Subtotal = d.Subtotal
        }).ToList()
    };
}
