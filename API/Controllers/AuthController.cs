using API.Services;
using API.DTOs;
using API.Models;
using API.Data;
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

        var user = await _authService.RegisterUserAsync(userDto.Username, userDto.Password);

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

        // Now using the updated VerifyPassword method
        bool isValidPassword = _authService.VerifyPassword(loginDto.Password, user);
        if (!isValidPassword)
        {
            return Unauthorized("Invalid username or password");
        }

        return Ok(new { userId = user.UserId, username = user.Username, isAdmin = user.IsAdmin });
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
