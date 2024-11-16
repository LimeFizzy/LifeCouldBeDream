using System;
using API.Data;
using System.IO;
using API.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class PictureUploadService : IPictureUploadService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PictureUploadService> _logger;

        public PictureUploadService(AppDbContext context, ILogger<PictureUploadService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> UploadProfileImageAsync(int userId, IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
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
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access to the file path {FilePath} is denied for user ID {UserId}.", filePath, userId);
                throw new UnauthorizedAccessException("Access to the file path is denied.", ex);
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
            if (!System.IO.File.Exists(imagePath))
            {
                _logger.LogWarning("Profile image file not found for user ID {UserId} at path {ImagePath}.", userId, imagePath);
                throw new FileNotFoundException("Image file not found.");
            }

            try
            {
                return await System.IO.File.ReadAllBytesAsync(imagePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access to the file path {ImagePath} is denied for user ID {UserId}.", imagePath, userId);
                throw new UnauthorizedAccessException("Access to the file path is denied.", ex);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the profile image for user ID {UserId}.", userId);
                throw new IOException("An error occurred while reading the profile image.", ex);
            }
        }
    }
}
