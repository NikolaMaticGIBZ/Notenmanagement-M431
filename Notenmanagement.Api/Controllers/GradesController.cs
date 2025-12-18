using Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Notenmanagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradesService;

    public GradesController(IGradeService gradesService)
    {
        _gradesService = gradesService;
    }

    // CREATE a grade (teacher_id comes from logged-in user)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGradeRequest request)
    {
        // TODO: replace with actual logged-in user id from JWT
        int teacherId = 1;

        var result = await _gradesService.CreateGradeAsync(request, teacherId);
        return Ok(result);
    }

    // GET all grades
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _gradesService.GetAllGradesAsync();
        return Ok(result);
    }

    // GET a single grade by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var grade = await _gradesService.GetAllGradesAsync();
        var singleGrade = grade.FirstOrDefault(g => g.Id == id);
        if (singleGrade == null)
            return NotFound();
        return Ok(singleGrade);
    }

    // DELETE a grade
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _gradesService.DeleteGradeAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
