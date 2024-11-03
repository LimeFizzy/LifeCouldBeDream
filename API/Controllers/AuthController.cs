using API.Interfaces;
using API.DTOs;
using API.Models;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AppDbContext context, IAuthService authService) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IAuthService _authService = authService;

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

            var hashedPassword = _authService.HashPassword(userDto.Password);

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(actionName: nameof(GetUserById), routeValues: new { id = user.UserId }, value: user); // 4. Used Named Spaces
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null)
            {
                return Unauthorized("User doesn't exist");
            }

            bool isValidPassword = _authService.VerifyPassword(loginDto.Password, user.PasswordHash);
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
}
