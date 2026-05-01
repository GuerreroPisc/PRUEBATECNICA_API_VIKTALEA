using ApiVIKTALEA.Data;
using ApiVIKTALEA.DTOs.Common;
using ApiVIKTALEA.DTOs.Productos;
using ApiVIKTALEA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiVIKTALEA.Services;

public class ProductoService : IProductoService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContext;

    public ProductoService(AppDbContext context, IHttpContextAccessor httpContext)
    {
        _context = context;
        _httpContext = httpContext;
    }

    private string UsuarioActual =>
        _httpContext.HttpContext?.User?.Identity?.Name ?? "system";

    public async Task<PagedResultDto<ProductoDto>> GetAllAsync(ProductoFilterDto filter)
    {
        var query = _context.Productos.Include(p => p.Categoria).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Nombre))
            query = query.Where(p => p.Nombre.Contains(filter.Nombre));
        if (filter.CategoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == filter.CategoriaId);
        if (filter.Estado.HasValue)
            query = query.Where(p => p.Estado == filter.Estado);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Nombre)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Stock = p.Stock,
                CategoriaId = p.CategoriaId,
                CategoriaNombre = p.Categoria.Nombre,
                Estado = p.Estado
            })
            .ToListAsync();

        return new PagedResultDto<ProductoDto>
        {
            Items = items,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ProductoDto?> GetByIdAsync(int id)
    {
        var p = await _context.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
        if (p is null) return null;
        return new ProductoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            Stock = p.Stock,
            CategoriaId = p.CategoriaId,
            CategoriaNombre = p.Categoria.Nombre,
            Estado = p.Estado
        };
    }

    public async Task<(ProductoDto? producto, string? error)> CreateAsync(CreateProductoDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return (null, "La categoría especificada no existe.");
        if (!categoria.Estado)
            return (null, "No se puede asignar un producto a una categoría inactiva.");

        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            Stock = dto.Stock,
            CategoriaId = dto.CategoriaId,
            Estado = true,
            UsuarioCreacion = UsuarioActual,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        return (new ProductoDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            Stock = producto.Stock,
            CategoriaId = producto.CategoriaId,
            CategoriaNombre = categoria.Nombre,
            Estado = producto.Estado
        }, null);
    }

    public async Task<(ProductoDto? producto, string? error)> UpdateAsync(int id, UpdateProductoDto dto)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto is null)
            return (null, "Producto no encontrado.");

        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return (null, "La categoría especificada no existe.");
        if (!categoria.Estado)
            return (null, "No se puede asignar un producto a una categoría inactiva.");

        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;
        producto.CategoriaId = dto.CategoriaId;
        producto.UsuarioModificacion = UsuarioActual;
        producto.FechaModificacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (new ProductoDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            Stock = producto.Stock,
            CategoriaId = producto.CategoriaId,
            CategoriaNombre = categoria.Nombre,
            Estado = producto.Estado
        }, null);
    }

    public async Task<(bool success, string? error)> DeactivateAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto is null)
            return (false, "Producto no encontrado.");
        if (!producto.Estado)
            return (false, "El producto ya está inactivo.");

        producto.Estado = false;
        producto.UsuarioEliminacion = UsuarioActual;
        producto.FechaEliminacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, null);
    }

    // Valida disponibilidad y descuenta stock en memoria (sin SaveChanges).
    // El llamador es responsable de persistir junto con su propia operación.
    public async Task<(bool success, string? error, decimal precio, string nombre)> ValidarYDescontarStockAsync(int productoId, int cantidad)
    {
        var producto = await _context.Productos.FindAsync(productoId);
        if (producto is null)
            return (false, $"El producto con Id {productoId} no existe.", 0, string.Empty);
        if (!producto.Estado)
            return (false, $"El producto '{producto.Nombre}' está inactivo y no puede agregarse a una orden.", 0, string.Empty);
        if (producto.Stock < cantidad)
            return (false, $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {cantidad}.", 0, string.Empty);

        producto.Stock -= cantidad;
        return (true, null, producto.Precio, producto.Nombre);
    }
}
