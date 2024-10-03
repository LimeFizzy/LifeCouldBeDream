using API.Services;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(AppDbContext context, AuthService authService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly AuthService _authService = authService;

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto userDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
        {
            return BadRequest("Email is already registered");
        }

        if (string.IsNullOrEmpty(userDto.Password))
        {
            return BadRequest("Password cannot be empty.");
        }

        var hashedPassword = _authService.HashPassword(userDto.Password);

        var user = new User
        {
            Name = userDto.Name,
            Email = userDto.Email,
            PasswordHash = hashedPassword
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password");
        }

        var isValidPassword = _authService.VerifyPassword(loginDto.Password, user.PasswordHash);
        if (!isValidPassword)
        {
            return Unauthorized("Invalid email or password");
        }

        return Ok(user.Name);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
}