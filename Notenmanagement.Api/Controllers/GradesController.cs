using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Notenmanagement.Api.Controllers;

/// <summary>
/// Authorized Controller for grades
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradesService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradesController"/> class.
    /// </summary>
    /// <param name="gradesService">The grades service.</param>
    public GradesController(IGradeService gradesService)
    {
        _gradesService = gradesService;
    }

    /// <summary>
    /// Creates a grade as the teacher.
    /// </summary>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns a Ok if sucessful</returns>
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

    /// <summary>
    /// Gets all grades if rektor.
    /// </summary>
    /// <param name="status">The status => pending / approved etc.</param>
    /// <returns>Returns grades in Ok</returns>
    [HttpGet]
    [Authorize(Roles = "rektor")]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var result = await _gradesService.GetGradesAsync(status);
        return Ok(result);
    }

    /// <summary>
    /// Gets a grade by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Returns the specific grade in a Ok if sucessful 
    ///          Else a NotFound if grade isnt found
    /// </returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "rektor")]
    public async Task<IActionResult> GetById(int id)
    {
        var grade = await _gradesService.GetByIdAsync(id);
        if (grade == null) return NotFound();
        return Ok(grade);
    }

    /// <summary>
    /// Deletes a grade with identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Returns a NotFound if grade isnt found
    ///          Else a NoContent if succesful
    /// </returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "rektor")]
    public async Task<IActionResult> Delete(int id)
    {
        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub") ??
            User.FindFirstValue("id");

        if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var rektorId))
            return Unauthorized("JWT does not contain a valid user id (sub).");

        var success = await _gradesService.DeleteGradeAsync(id, rektorId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Decision as a rektor if grade is approved or not.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns a NotFound id grade couldnt be found by identifier
    ///          Else if succesful a NoContent
    /// </returns>    
    [HttpPatch("{id:int}/decision")]
    [Authorize(Roles = "rektor")]
    public async Task<IActionResult> Decide(int id, [FromBody] DecideGradeRequest request)
    {
        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub") ??
            User.FindFirstValue("id");

        if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var rektorId))
            return Unauthorized("JWT does not contain a valid user id (sub).");

        var success = await _gradesService.DecideAsync(id, rektorId, request.Status, request.DecisionNote);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Gets teachers requests.
    /// </summary>
    /// <returns>Returns a Unauthorized if nothing is in Jwt
    ///          Else a Ok with result if succesful
    /// </returns>
    [HttpGet("mine")]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> GetMine()
    {
        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub") ??
            User.FindFirstValue("id");

        if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var teacherId))
            return Unauthorized("JWT does not contain a valid user id (sub).");

        var result = await _gradesService.GetMyGradesAsync(teacherId);
        return Ok(result);
    }

    /// <summary>
    /// Updates a request from teacher if rektor didnt approve it yet.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns a Unathorized if nothing is in Jwt, else a NotFound if grade isnt found
    ///          and if succesful a NoContent
    /// </returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> UpdateMine(int id, [FromBody] UpdateGradeRequest request)
    {
        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub") ??
            User.FindFirstValue("id");

        if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var teacherId))
            return Unauthorized("JWT does not contain a valid user id (sub).");

        var ok = await _gradesService.UpdateMyGradeAsync(id, teacherId, request);
        if (!ok) return NotFound(); // not found OR not owned OR not pending
        return NoContent();
    }

    /// <summary>
    /// Deletes a request from teachers request if rektor didnt approve it yet.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Returns a Unauthorized if nothing is in Jwt
    ///          Else if grade isnt found a NotFound
    ///          If succesful a NoContent
    /// </returns>
    [HttpDelete("mine/{id:int}")]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> DeleteMine(int id)
    {
        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub") ??
            User.FindFirstValue("id");

        if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var teacherId))
            return Unauthorized("JWT does not contain a valid user id (sub).");

        var ok = await _gradesService.DeleteMyGradeAsync(id, teacherId);
        if (!ok) return NotFound(); // not found OR not owned OR not pending
        return NoContent();
    }
}
