using Api.DataAccess.Interfaces;
using Api.DataAccess.Models;
using Api.Services.Interfaces;
using Shared.DTOs;

namespace Api.Services.Services;

/// <summary>
/// Service for Authentication
/// </summary>
/// <seealso cref="Api.Services.Interfaces.IAuthService" />
public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="authRepository">The authentication repository.</param>
    public AuthService(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    /// <inheritdoc/>
    public async Task<User?> RegisterAsync(RegisterRequest request)
    {
        return await _authRepository.RegisterAsync(request);
    }

    /// <inheritdoc/>
    public async Task<User?> LoginAsync(LoginRequest request)
    {
        return await _authRepository.LoginAsync(request);
    }
}