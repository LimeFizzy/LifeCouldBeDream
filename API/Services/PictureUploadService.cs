using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using API.Data;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using System;
using System.Collections.Generic;

namespace API.Services
{
    public class PictureUploadService : IPictureUploadService
    {
        private readonly AppDbContext _context;

        public PictureUploadService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> UploadProfileImageAsync(int userId, IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                throw new ArgumentException("No file uploaded.");
            }

            var user = await _context.Users.FindAsync(userId) ?? throw new KeyNotFoundException("User not found.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{userId}_{Path.GetFileName(profileImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create)) // 7. Using a stream to load data
                {
                    await profileImage.CopyToAsync(stream);
                }

                user.ProfileImagePath = filePath;
                await _context.SaveChangesAsync();
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException("Access to the file path is denied.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException("An error occurred while saving the profile image.", ex);
            }

            return filePath;
        }

        public async Task<byte[]> GetProfileImageAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId) ?? throw new KeyNotFoundException("User or profile image not found.");

            if (string.IsNullOrEmpty(user.ProfileImagePath))
            {
                throw new FileNotFoundException("Profile image path is missing.");
            }

            var imagePath = user.ProfileImagePath;
            if (!System.IO.File.Exists(imagePath))
            {
                throw new FileNotFoundException("Image file not found.");
            }

            try
            {
                return await System.IO.File.ReadAllBytesAsync(imagePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException("Access to the file path is denied.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException("An error occurred while reading the profile image.", ex);
            }
        }
    }
}
