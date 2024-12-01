using API.Data;
using API.Interfaces;

namespace API.Services
{
    public class PictureUploadService(AppDbContext context, ILogger<PictureUploadService> logger) : IPictureUploadService
    {
        private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<PictureUploadService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> UploadProfileImageAsync(int userId, IFormFile profileImage)
        {
            if (profileImage == null)
            {
                _logger.LogWarning("No file uploaded for user ID {UserId}.", userId);
                throw new ArgumentException("No file uploaded.");
            }

            var user = await _context.Users.FindAsync(userId)
                       ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                _logger.LogInformation("Created uploads directory at {UploadsFolder}.", uploadsFolder);
            }

            var fileName = $"{userId}_{Path.GetFileName(profileImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                user.ProfileImagePath = filePath;
                await _context.SaveChangesAsync();
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "An error occurred while saving the profile image for user ID {UserId}.", userId);
                throw new IOException("An error occurred while saving the profile image.", ex);
            }

            return filePath;
        }

        public async Task<byte[]> GetProfileImageAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId)
                       ?? throw new KeyNotFoundException($"User with ID {userId} or their profile image was not found.");

            if (string.IsNullOrEmpty(user.ProfileImagePath))
            {
                _logger.LogWarning("Profile image path is missing for user ID {UserId}.", userId);
                throw new FileNotFoundException("Profile image path is missing.");
            }

            var imagePath = user.ProfileImagePath;
            if (!File.Exists(imagePath))
            {
                _logger.LogWarning("Profile image file not found for user ID {UserId} at path {ImagePath}.", userId, imagePath);
                throw new FileNotFoundException("Image file not found.");
            }

            try
            {
                return await File.ReadAllBytesAsync(imagePath);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the profile image for user ID {UserId}.", userId);
                throw new IOException("An error occurred while reading the profile image.", ex);
            }
        }
    }
}
