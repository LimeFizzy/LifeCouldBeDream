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
    public async Task<ActionResult<User>> Register([FromBody] UserDto userDto)
    {
        var existingUser = await _context.Users.AnyAsync(u => u.Username == userDto.Username);
        if (existingUser)
        {
            return BadRequest(error: "Username is already taken.");     // 4. Used Named Spaces
        }

        if (string.IsNullOrEmpty(userDto.Password))
        {
            return BadRequest(error: "Password cannot be empty.");      // 4. Used Named Spaces
        }

        var user = await _authService.RegisterUserAsync(username: userDto.Username, password: userDto.Password);        // 4. Used Named Spaces

        return CreatedAtAction(actionName: nameof(GetUserById), routeValues: new { id = user.UserId }, value: user);        // 4. Used Named Spaces
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

        return Ok(new { userId = user.UserId, username = user.Username });
    }

    [HttpGet("is-admin/{username}")]
    public async Task<IActionResult> IsAdmin(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return NotFound("User not found.");

        return Ok(new { IsAdmin = user.IsAdmin });
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
