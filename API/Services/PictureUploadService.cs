using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class PictureUploadService(AppDbContext context) : IPictureUploadService
    {
        private readonly AppDbContext _context = context;

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

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            user.ProfileImagePath = filePath;
            await _context.SaveChangesAsync();

            return filePath;
        }

        public async Task<byte[]> GetProfileImageAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.ProfileImagePath))
            {
                throw new KeyNotFoundException("User or profile image not found.");
            }

            var imagePath = user.ProfileImagePath;
            if (!System.IO.File.Exists(imagePath))
            {
                throw new FileNotFoundException("Image file not found.");
            }

            return await System.IO.File.ReadAllBytesAsync(imagePath);
        }
    }
}
