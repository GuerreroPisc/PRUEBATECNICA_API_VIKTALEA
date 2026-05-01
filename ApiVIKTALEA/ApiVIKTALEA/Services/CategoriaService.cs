using ApiVIKTALEA.Data;
using ApiVIKTALEA.DTOs.Categorias;
using ApiVIKTALEA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiVIKTALEA.Services;

public class CategoriaService : ICategoriaService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContext;

    public CategoriaService(AppDbContext context, IHttpContextAccessor httpContext)
    {
        _context = context;
        _httpContext = httpContext;
    }

    private string UsuarioActual =>
        _httpContext.HttpContext?.User?.Identity?.Name ?? "system";

    public async Task<List<CategoriaDto>> GetAllAsync()
    {
        return await _context.Categorias
            .Select(c => new CategoriaDto { Id = c.Id, Nombre = c.Nombre, Estado = c.Estado })
            .ToListAsync();
    }

    public async Task<CategoriaDto?> GetByIdAsync(int id)
    {
        var c = await _context.Categorias.FindAsync(id);
        if (c is null) return null;
        return new CategoriaDto { Id = c.Id, Nombre = c.Nombre, Estado = c.Estado };
    }

    public async Task<CategoriaDto> CreateAsync(CreateCategoriaDto dto)
    {
        var categoria = new Categoria
        {
            Nombre = dto.Nombre,
            Estado = true,
            UsuarioCreacion = UsuarioActual,
            FechaCreacion = DateTime.UtcNow
        };
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return new CategoriaDto { Id = categoria.Id, Nombre = categoria.Nombre, Estado = categoria.Estado };
    }

    public async Task<CategoriaDto?> UpdateAsync(int id, UpdateCategoriaDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null) return null;

        categoria.Nombre = dto.Nombre;
        categoria.UsuarioModificacion = UsuarioActual;
        categoria.FechaModificacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return new CategoriaDto { Id = categoria.Id, Nombre = categoria.Nombre, Estado = categoria.Estado };
    }

    public async Task<(bool success, string? error)> DeactivateAsync(int id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria is null)
            return (false, "Categoría no encontrada.");
        if (!categoria.Estado)
            return (false, "La categoría ya está inactiva.");
        if (categoria.Productos.Any(p => p.Estado))
            return (false, "No se puede desactivar la categoría porque tiene productos activos asociados.");

        categoria.Estado = false;
        categoria.UsuarioEliminacion = UsuarioActual;
        categoria.FechaEliminacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, null);
    }
}
