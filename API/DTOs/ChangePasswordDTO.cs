namespace API.DTOs
{
    public class ChangePasswordDto
    {
        public required string Username { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
