using Api.DataAccess.Models;
using Shared.DTOs;

namespace Api.DataAccess.Interfaces;

/// <summary>
/// Interface for Authentication Repository
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Regsiters new user and puts him in database asynchronous.
    /// </summary>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns new user</returns>
    Task<User?> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Login a user and checking if is same as in database asynchronous.
    /// </summary>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns user</returns>
    Task<User?> LoginAsync(LoginRequest request);

    /// <summary>
    /// Gets a user by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Retuns the found user</returns>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// Saves changes asynchronous.
    /// </summary>
    /// <returns>Empty Task</returns>
    Task SaveAsync();
}