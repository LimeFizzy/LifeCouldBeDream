using Serilog;
using API.DTOs;
using API.Data;
using ZstdSharp;
using API.Models;
using API.Exceptions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, IAuthService authService, ILogger<AuthController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                _logger.LogWarning("Attempt to register with null user data.");
                throw new ArgumentNullException(nameof(userDto), "User data is required.");
            }

            try
            {
                var existingUser = await _context.Users.AnyAsync(u => u.Username == userDto.Username);
                if (existingUser)
                {
                    _logger.LogWarning("Username '{Username}' is already taken.", userDto.Username);
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
                _logger.LogWarning("Weak password provided for user '{Username}': {ErrorMessage}", userDto.Username, ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Null argument encountered during registration.");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during registration.");
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                _logger.LogWarning("Attempt to login with null login data.");
                throw new ArgumentNullException(nameof(loginDto), "Login data is required.");
            }


            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed. User '{Username}' does not exist.", loginDto.Username);
                return NotFound(new { Message = "User does not exist." });
            }

            bool isValidPassword = _authService.VerifyPassword(loginDto.Password, user.PasswordHash);
            if (!isValidPassword)
            {
                _logger.LogWarning("Invalid password for user '{Username}'.", loginDto.Username);
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            return Ok(new { userId = user.UserId, username = user.Username });
        }

        [HttpGet("is-admin/{username}")]
        public async Task<IActionResult> IsAdmin(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("IsAdmin check failed. Username cannot be empty.");
                return BadRequest(new { Message = "Username cannot be empty." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                _logger.LogWarning("User '{Username}' not found for admin check.", username);
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
                _logger.LogWarning("User with ID '{UserId}' not found.", id);
                return NotFound(new { Message = "User not found." });
            }

            return user;
        }
    }
}
