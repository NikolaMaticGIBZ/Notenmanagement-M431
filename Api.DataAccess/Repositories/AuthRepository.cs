using Api.DataAccess;
using Api.DataAccess.Interfaces;
using Api.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using System.Security.Cryptography;
using System.Text;


public class AuthRepository : IAuthRepository
{
    private readonly AppDBContext _context;

    public AuthRepository(AppDBContext context)
    {
        _context = context;
    }

    public async Task<Users?> RegisterAsync(RegisterRequest request)
    {
        bool exists = await _context.Users.AnyAsync(u =>
            u.email == request.Email || u.username == request.Username);

        if (exists)
            return null;

        var user = new Users
        {
            username = request.Username,
            email = request.Email,
            password_hash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<Users?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.email == request.Email);

        if (user == null)
            return null;

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.password_hash
        );

        if (!validPassword)
            return null;
        return user;
    }
    public async Task<Users?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.id == id);
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
