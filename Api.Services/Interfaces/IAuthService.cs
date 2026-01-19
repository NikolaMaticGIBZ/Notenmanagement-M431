using Api.DataAccess.Models;
using Shared.DTOs;

namespace Api.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Register a user asynchronous.
    /// </summary>
    /// <param name="request">The request dto (username, email and password).</param>
    /// <returns>Awaits the register method from the repository</returns>
    Task<User?> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Logins a user asynchronous.
    /// </summary>
    /// <param name="request">The request dto (email and password).</param>
    /// <returns>Awaits the login method from the repository</returns>
    Task<User?> LoginAsync(LoginRequest request);
}
