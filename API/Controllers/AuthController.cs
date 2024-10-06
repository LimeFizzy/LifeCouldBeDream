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

        var existingUser = await _context.Users.AnyAsync(u => u.Username == userDto.Username);
        if (existingUser)
        {
            return BadRequest("Username is already taken.");
        }

        if (string.IsNullOrEmpty(userDto.Password))
        {
            return BadRequest("Password cannot be empty.");
        }

        var hashedPassword = _authService.HashPassword(userDto.Password);

        var user = new User
        {
            Username = userDto.Username,
            PasswordHash = hashedPassword
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        if (user == null)
        {
            return Unauthorized("User doesn't exist");
        }

        var isValidPassword = _authService.VerifyPassword(loginDto.Password, user.PasswordHash);
        if (!isValidPassword)
        {
            return Unauthorized("Invalid email or password");
        }

        return Ok(new { userId = user.UserId, username = user.Username });
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