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
    public class AuthController(AppDbContext context, IAuthService authService) : ControllerBase
    {
        private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDto userDto)
        {
            try
            {
                var existingUser = await _context.Users.AnyAsync(u => u.Username == userDto.Username);
                if (existingUser)
                {
                    throw new ArgumentException("Username is already taken.", nameof(userDto.Username));
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

                return CreatedAtAction(actionName: nameof(GetUserById), routeValues: new { id = user.UserId }, value: user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (WeakPasswordException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("User does not exist.");
                }

                bool isValidPassword = _authService.VerifyPassword(loginDto.Password, user.PasswordHash);
                if (!isValidPassword)
                {
                    throw new UnauthorizedAccessException("Invalid username or password.");
                }

                return Ok(new { userId = user.UserId, username = user.Username });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while logging in." });
            }
        }

        [HttpGet("is-admin/{username}")]
        public async Task<IActionResult> IsAdmin(string username)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                return Ok(new { IsAdmin = user.IsAdmin });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while checking admin status." });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                return user;
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while fetching the user." });
            }
        }
    }
}
