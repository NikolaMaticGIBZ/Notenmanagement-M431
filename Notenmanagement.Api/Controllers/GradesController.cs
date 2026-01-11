using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Notenmanagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradesService;

    public GradesController(IGradeService gradesService)
    {
        _gradesService = gradesService;
    }

    // CREATE a grade (teacher_id comes from logged-in user)
    [HttpPost]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> Create([FromBody] CreateGradeRequest request)
    {
        // TODO: replace with actual logged-in user id from JWT || Done!!
        int teacherId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        string role = User.FindFirstValue(ClaimTypes.Role)!;

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
