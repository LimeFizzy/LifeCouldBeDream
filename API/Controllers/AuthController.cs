using API.Interfaces;
using API.DTOs;
using API.Models;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Exceptions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public AuthController(AppDbContext context, IAuthService authService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto), "User data is required.");
            }

            try
            {
                var existingUser = await _context.Users.AnyAsync(u => u.Username == userDto.Username);
                if (existingUser)
                {
                    return BadRequest(new { Message = "Username is already taken." });
                }

                _authService.ValidatePasswordStrength(userDto.Password);

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
            catch (WeakPasswordException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto), "Login data is required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null)
            {
                return NotFound(new { Message = "User does not exist." });
            }

            bool isValidPassword = _authService.VerifyPassword(loginDto.Password, user.PasswordHash);
            if (!isValidPassword)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            return Ok(new { userId = user.UserId, username = user.Username });
        }

        [HttpGet("is-admin/{username}")]
        public async Task<IActionResult> IsAdmin(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { Message = "Username cannot be empty." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(new { IsAdmin = user.IsAdmin });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return user;
        }
    }
}
