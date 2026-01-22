using Api.DataAccess.Interfaces;
using Api.Services.Interfaces;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using System.Security.Cryptography;

namespace Notenmanagement.Api.Controllers;

/// <summary>
/// Controller for Authentication
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IEmailService _emailService;
    private readonly JwtService _jwtService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authRepository">The authentication repository.</param>
    /// <param name="emailService">The email service.</param>
    /// <param name="jwtService">The JWT service.</param>
    public AuthController(IAuthRepository authRepository, IEmailService emailService, JwtService jwtService)
    {
        _authRepository = authRepository;
        _emailService = emailService;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Post request for registering with username, email and password.
    /// </summary>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns a Ok if succesful else a Bad Request</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var email = request.Email.ToLower().Trim();
        string role = string.Empty;

        if (email.EndsWith("@gibz.ch"))
        {
            role = "teacher";
        }
        else if (email.EndsWith("@zg.ch"))
        {
            role = "rektor";
        }
        else if (email.EndsWith("@hotmail.com")) // @hotmail.com for testing purposes
        {
            role = "teacher";
        }
        else
        {
            return BadRequest("Nur E-Mail-Adressen mit @gibz.ch oder @zg.ch sind erlaubt.");
        }
        request.Role = role;

        var user = await _authRepository.RegisterAsync(request);
        if (user == null)
            return BadRequest("Benutzer existiert bereits");

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role
        });
    }

    /// <summary>
    /// Post request for logging in with email and password.
    /// </summary>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns a Ok if succesful else unauthorized</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _authRepository.LoginAsync(request);
        if (user == null)
            return Unauthorized("Ungültige Anmeldedaten");

        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        user.Twofactor_code = code;
        user.Twofactor_expires = DateTime.UtcNow.AddMinutes(5);
        user.Twofactor_verified = false;

        await _authRepository.SaveAsync();

        await _emailService.SendAsync(
            user.Email,
            "Ihr Sicherheitscode",
            $"Ihr 2FA-Code lautet: {code}\nGültig für 5 Minuten."
        );

        
        Console.WriteLine($"[2FA CODE für {user.Email}]: {code}");

        return Ok(new
        {
            requiresTwoFactor = true,
            userId = user.Id
        });
    }

    /// <summary>
    /// Post request with 2fa code.
    /// </summary>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns Ok if succesful
    ///          If alredy verified a BadRequest
    ///          Else a Unauthorized
    /// </returns>
    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactor(Verify2FARequest request)
    {
        var user = await _authRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return Unauthorized("User not found");

        if (user.Twofactor_verified)
            return BadRequest("2FA already verified");

        if (user.Twofactor_expires == null || user.Twofactor_expires < DateTime.UtcNow)
            return Unauthorized("2FA code expired");

        if (user.Twofactor_code != request.Code)
            return Unauthorized("Invalid 2FA code");

        // 2FA erfolgreich
        user.Twofactor_verified = true;
        user.Twofactor_code = null;
        user.Twofactor_expires = null;
        await _authRepository.SaveAsync();

        // JWT generieren
        var token = _jwtService.GenerateToken(user, user.Role);

        return Ok(new
        {
            success = true,
            token,
            role = user.Role
        });
    }

    /// <summary>
    /// Post request for 2FA just this time if resending.
    /// </summary>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns a NotFound if user not found
    ///          If succesful a Ok
    /// </returns>
    [HttpPost("resend-2fa")]
    public async Task<IActionResult> ResendTwoFactor([FromBody] Resend2FARequest request)
    {
        var user = await _authRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return NotFound("User not found");

        // Neuen 6-stelligen Code generieren
        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        user.Twofactor_code = code;
        user.Twofactor_expires = DateTime.UtcNow.AddMinutes(5);
        user.Twofactor_verified = false;

        await _authRepository.SaveAsync();

        // Code per E-Mail senden
        await _emailService.SendAsync(
            user.Email,
            "Ihr Sicherheitscode",
            $"Ihr neuer 2FA-Code lautet: {code}\nGültig für 5 Minuten."
        );

        Console.WriteLine($"[Resend 2FA CODE für {user.Email}]: {code}");

        return Ok(new
        {
            success = true,
            message = "2FA-Code wurde erneut gesendet."
        });
    }
}