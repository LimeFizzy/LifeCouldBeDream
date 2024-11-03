using Microsoft.AspNetCore.Mvc;
using API.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PictureUploadController(IPictureUploadService userService) : ControllerBase
    {
        private readonly IPictureUploadService _userService = userService;

        [HttpPost("upload-profile-image/{userId}")]
        public async Task<IActionResult> UploadProfileImage(int userId, IFormFile profileImage)
        {
            try
            {
                var filePath = await _userService.UploadProfileImageAsync(userId, profileImage);
                return Ok(new { Message = "Profile image uploaded successfully.", ProfileImagePath = filePath });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}/profile-image")]
        public async Task<IActionResult> GetProfileImage(int userId)
        {
            try
            {
                var fileBytes = await _userService.GetProfileImageAsync(userId);
                return File(fileBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
