using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PictureUploadController : ControllerBase
    {
        private readonly IPictureUploadService _userService;

        public PictureUploadController(IPictureUploadService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("upload-profile-image/{userId}")]
        public async Task<IActionResult> UploadProfileImage(int userId, IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                return BadRequest(new { Message = "Profile image cannot be null or empty." });
            }

            try
            {
                var filePath = await _userService.UploadProfileImageAsync(userId, profileImage);
                return Ok(new { Message = "Profile image uploaded successfully.", ProfileImagePath = filePath });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle cases where access is denied
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle invalid arguments, such as an invalid userId or unsupported format
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                // Handle cases where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Generic catch for unexpected errors
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
                    return NotFound(new { Message = "Profile image not found." });
                }

                return File(fileBytes, "image/jpeg");
            }
            catch (FileNotFoundException ex)
            {
                // Handle explicitly not found files
                return NotFound(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                // Handle cases where user or profile image not found
                return NotFound(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle cases where access is denied
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Generic catch for unexpected errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving the profile image.", Error = ex.Message });
            }
        }
    }
}
