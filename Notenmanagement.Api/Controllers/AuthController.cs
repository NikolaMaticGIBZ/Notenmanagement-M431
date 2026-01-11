using Api.DataAccess.Interfaces;
using Api.Services.Interfaces;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using System.Security.Cryptography;

namespace Notenmanagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IEmailService _emailService;
    private readonly JwtService _jwtService;


    public AuthController(IAuthRepository authRepository, IEmailService emailService, JwtService jwtService)
    {
        _authRepository = authRepository;
        _emailService = emailService;
        _jwtService = jwtService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = await _authRepository.RegisterAsync(request);
        if (user == null)
            return BadRequest("User already exists");

        return Ok(new
        {
            user.id,
            user.username,
            user.email
        });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _authRepository.LoginAsync(request);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        user.twofactor_code = code;
        user.twofactor_expires = DateTime.UtcNow.AddMinutes(5);
        user.twofactor_verified = false;

        await _authRepository.SaveAsync();

        await _emailService.SendAsync(
            user.email,
            "Ihr Sicherheitscode",
            $"Ihr 2FA-Code lautet: {code}\nGültig für 5 Minuten."
        );

        // TODO: per Mail senden
        Console.WriteLine($"[2FA CODE für {user.email}]: {code}");

        return Ok(new
        {
            requiresTwoFactor = true,
            userId = user.id
        });
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactor(Verify2FARequest request)
    {
        var user = await _authRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return Unauthorized("User not found");

        if (user.twofactor_verified)
            return BadRequest("2FA already verified");

        if (user.twofactor_expires == null || user.twofactor_expires < DateTime.UtcNow)
            return Unauthorized("2FA code expired");

        if (user.twofactor_code != request.Code)
            return Unauthorized("Invalid 2FA code");

        // 2FA erfolgreich
        user.twofactor_verified = true;
        user.twofactor_code = null;
        user.twofactor_expires = null;
        await _authRepository.SaveAsync();

        // JWT generieren
        var token = _jwtService.GenerateToken(user, "teacher"); // Rolle aus DB später

        return Ok(new
        {
            success = true,
            token,
            role = "teacher"
        });
    }
}
