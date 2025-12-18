using Api.DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Notenmanagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = await _authRepository.RegisterAsync(request);
        if (user == null)
            return BadRequest("User already exists");

        return Ok(new { user.id, user.username, user.email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _authRepository.LoginAsync(request);
        if (user == null)
            return Unauthorized("Invalid credentials");

        return Ok(new { user.id, user.username, user.email });
    }
}
