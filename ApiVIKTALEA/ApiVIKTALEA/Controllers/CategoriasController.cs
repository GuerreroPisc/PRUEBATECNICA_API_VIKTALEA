using ApiVIKTALEA.DTOs.Categorias;
using ApiVIKTALEA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiVIKTALEA.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _service;

    public CategoriasController(ICategoriaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null)
            return NotFound(new { message = "Categoría no encontrada." });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result is null)
            return NotFound(new { message = "Categoría no encontrada." });
        return Ok(result);
    }

    [HttpPatch("{id:int}/desactivar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var (success, error) = await _service.DeactivateAsync(id);
        if (!success)
            return error == "Categoría no encontrada."
                ? NotFound(new { message = error })
                : BadRequest(new { message = error });
        return NoContent();
    }

    [HttpPatch("{id:int}/activar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Activate(int id)
    {
        var (success, error) = await _service.ActivateAsync(id);
        if (!success)
            return error == "Categoría no encontrada."
                ? NotFound(new { message = error })
                : BadRequest(new { message = error });
        return NoContent();
    }
}
