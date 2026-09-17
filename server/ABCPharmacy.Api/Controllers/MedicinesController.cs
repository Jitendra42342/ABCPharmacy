using ABCPharmacy.Api.DTOs;
using ABCPharmacy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCPharmacy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MedicinesController(MedicineService service) : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] string? search) =>
        Ok(service.Get(search));

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var medicine = service.GetById(id);
        return medicine is null ? NotFound(new { message = "Medicine not found." }) : Ok(medicine);
    }

    [HttpPost]
    public IActionResult Create(CreateMedicineRequest request)
    {
        try
        {
            var medicine = service.Add(request);
            return CreatedAtAction(nameof(GetById), new { id = medicine.Id }, medicine);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
