using ApiVIKTALEA.DTOs.Ordenes;
using ApiVIKTALEA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiVIKTALEA.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdenesController : ControllerBase
{
    private readonly IOrdenService _service;

    public OrdenesController(IOrdenService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrdenFilterDto filter)
    {
        var result = await _service.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null)
            return NotFound(new { message = "Orden no encontrada." });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrdenDto dto)
    {
        var (orden, error) = await _service.CreateAsync(dto);
        if (orden is null)
            return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = orden.Id }, orden);
    }
}
