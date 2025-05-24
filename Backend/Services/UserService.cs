using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public UserService(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<string?> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return null;

        var hashedPassword = PasswordHasher.HashPassword(dto.Password);
        var user = new User { Username = dto.Username, PasswordHash = hashedPassword };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return _jwtService.GenerateToken(user.Id.ToString(), user.Username);
    }

    public async Task<string?> Login(LoginDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            return null;

        return _jwtService.GenerateToken(user.Id.ToString(), user.Username);
    }
}