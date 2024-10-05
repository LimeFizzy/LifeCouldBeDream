using Microsoft.AspNetCore.Mvc;
using API.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpPost("upload-profile-image/{userId}")]
        public async Task<IActionResult> UploadProfileImage(int userId, IFormFile profileImage)
        {
            // Check if the file is provided
            if (profileImage == null || profileImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Find the user in the database
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Create the directory to store uploads if it does not exist
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique file name
            var fileName = $"{userId}_{Path.GetFileName(profileImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save the file using stream
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            // Update user's profile image path in the database
            user.ProfileImagePath = filePath;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Profile image uploaded successfully.", ProfileImagePath = filePath });
        }

        [HttpGet("{userId}/profile-image")]
        public async Task<IActionResult> GetProfileImage(int userId)
        {
            // Find the user in the database
            var user = await _context.Users.FindAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.ProfileImagePath))
            {
                return NotFound("User or profile image not found.");
            }

            // Serve the image file
            var imagePath = user.ProfileImagePath;
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Image file not found.");
            }

            // Return the image file
            var fileBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
            return File(fileBytes, "image/jpeg"); // Change MIME type based on your image format
        }
    }
}
