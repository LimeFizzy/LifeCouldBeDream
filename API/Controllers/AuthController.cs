using API.DTOs;
using API.Data;
using API.Models;
using API.Exceptions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpDelete("delete/{username}")]
        public async Task<ActionResult<User>> Delete(string username)
        {
            if (username == null)
            {
                _logger.LogWarning("Attempt to delete with null user data.");
                throw new ArgumentNullException(username, "Username is required.");
            }

            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (existingUser == null)
                {
                    _logger.LogWarning("No such user in database '{username}'", username);
                    return BadRequest(new { Message = "Username does not exist" });
                }

                _context.Users.Remove(existingUser);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "User deleted successfully.", Username = username });
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

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null)
            {
                _logger.LogWarning("Attempt to change password with null data.");
                return BadRequest(new { Message = "Password change data is required." });
            }

            try
            {
                // Retrieve the user from the database using the username
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == changePasswordDto.Username);
                if (user == null)
                {
                    _logger.LogWarning("User '{Username}' not found for password change.", changePasswordDto.Username);
                    return NotFound(new { Message = "User not found." });
                }

                // Verify if the old password is correct
                bool isOldPasswordValid = _authService.VerifyPassword(changePasswordDto.OldPassword, user.PasswordHash);
                if (!isOldPasswordValid)
                {
                    _logger.LogWarning("Invalid old password for user '{Username}'.", changePasswordDto.Username);
                    return Unauthorized(new { Message = "Invalid old password." });
                }

                // Validate the new password's strength
                _authService.ValidatePasswordStrength(changePasswordDto.NewPassword);


                // Hash the new password
                var newHashedPassword = _authService.HashPassword(changePasswordDto.NewPassword);

                // Update the user's password
                user.PasswordHash = newHashedPassword;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during password change.");
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }

    }
}
