using Api.DataAccess;
using Api.DataAccess.Interfaces;
using Api.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

/// <summary>
/// Repository for Authentication
/// </summary>
public class AuthRepository : IAuthRepository
{
    private readonly AppDBContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthRepository"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public AuthRepository(AppDBContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<User?> RegisterAsync(RegisterRequest request)
    {
        bool exists = await _context.User.AnyAsync(u =>
            u.Email == request.Email || u.Username == request.Username);

        if (exists)
            return null;

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password_hash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <inheritdoc/>
    public async Task<User?> LoginAsync(LoginRequest request)
    {
        var user = await _context.User
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return null;

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.Password_hash
        );

        if (!validPassword)
            return null;
        return user;
    }

    /// <inheritdoc/>
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <inheritdoc/>
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}