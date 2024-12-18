using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PictureUploadController : ControllerBase
    {
        private readonly IPictureUploadService _userService;
        private readonly ILogger<PictureUploadController> _logger;

        public PictureUploadController(IPictureUploadService userService, ILogger<PictureUploadController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("upload-profile-image/{userId}")]
        public async Task<IActionResult> UploadProfileImage(int userId, IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                _logger.LogWarning("Profile image is null or empty for user {UserId}.", userId);
                return BadRequest(new { Message = "Profile image cannot be null or empty." });
            }

            try
            {
                var filePath = await _userService.UploadProfileImageAsync(userId, profileImage);

                return Ok(new { Message = "Profile image uploaded successfully.", ProfileImagePath = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while uploading profile image for user {UserId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while uploading the profile image.", Error = ex.Message });
            }
        }

        [HttpGet("{userId}/profile-image")]
        public async Task<IActionResult> GetProfileImage(int userId)
        {
            try
            {
                var fileBytes = await _userService.GetProfileImageAsync(userId);

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    _logger.LogWarning("Profile image not found for user {UserId}.", userId);
                    return NotFound(new { Message = "Profile image not found." });
                }

                return File(fileBytes, "image/jpeg");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Access denied while fetching profile image for user {UserId}.", userId);
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving profile image for user {UserId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving the profile image.", Error = ex.Message });
            }
        }
    }
}
