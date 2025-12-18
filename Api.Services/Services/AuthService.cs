using Api.DataAccess.Interfaces;
using Api.DataAccess.Models;
using Api.Services.Interfaces;
using Shared.DTOs;

namespace Api.Services.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;

    public AuthService(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Users?> RegisterAsync(RegisterRequest request)
    {
        return await _authRepository.RegisterAsync(request);
    }

    public async Task<Users?> LoginAsync(LoginRequest request)
    {
        return await _authRepository.LoginAsync(request);
    }
}
