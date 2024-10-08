namespace API.Services
{
    public interface IPictureUploadService
    {
        Task<string> UploadProfileImageAsync(int userId, IFormFile profileImage);
        Task<byte[]> GetProfileImageAsync(int userId);
    }
}
