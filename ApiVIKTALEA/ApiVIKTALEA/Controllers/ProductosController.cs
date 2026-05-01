using ApiVIKTALEA.DTOs.Productos;
using ApiVIKTALEA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiVIKTALEA.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _service;

    public ProductosController(IProductoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductoFilterDto filter)
    {
        var result = await _service.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null)
            return NotFound(new { message = "Producto no encontrado." });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateProductoDto dto)
    {
        var (producto, error) = await _service.CreateAsync(dto);
        if (producto is null)
            return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductoDto dto)
    {
        var (producto, error) = await _service.UpdateAsync(id, dto);
        if (producto is null)
            return error == "Producto no encontrado."
                ? NotFound(new { message = error })
                : BadRequest(new { message = error });
        return Ok(producto);
    }

    [HttpPatch("{id:int}/desactivar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var (success, error) = await _service.DeactivateAsync(id);
        if (!success)
            return error == "Producto no encontrado."
                ? NotFound(new { message = error })
                : BadRequest(new { message = error });
        return NoContent();
    }
}
