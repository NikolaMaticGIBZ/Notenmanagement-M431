using Api.DataAccess.Models;
using Shared.DTOs;

namespace Api.DataAccess.Interfaces;

public interface IAuthRepository
{
    Task<Users?> RegisterAsync(RegisterRequest request);
    Task<Users?> LoginAsync(LoginRequest request);

}
