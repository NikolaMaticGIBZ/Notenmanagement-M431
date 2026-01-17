using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> Create([FromBody] CreateGradeRequest request)
    {
        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub") ??
            User.FindFirstValue("id");

        if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var teacherId))
            return Unauthorized("JWT does not contain a valid user id (sub).");

        var result = await _gradesService.CreateGradeAsync(request, teacherId);
        return Ok(result);
    }



    [HttpGet]
    [Authorize(Roles = "rektor")]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var result = await _gradesService.GetGradesAsync(status);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "rektor")]
    public async Task<IActionResult> GetById(int id)
    {
        var grade = await _gradesService.GetByIdAsync(id);
        if (grade == null) return NotFound();
        return Ok(grade);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "rektor")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _gradesService.DeleteGradeAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id:int}/decision")]
    [Authorize(Roles = "rektor")]
    public async Task<IActionResult> Decide(int id, [FromBody] DecideGradeRequest request)
    {
        var success = await _gradesService.DecideAsync(id, request.Status, request.DecisionNote);
        if (!success) return NotFound();
        return NoContent();
    }
}
