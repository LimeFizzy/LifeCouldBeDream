using System.Text.Json.Serialization;

namespace API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsAdmin { get; set; } = false;

        [JsonIgnore]
        public ICollection<UserScore> Scores { get; set; } = new List<UserScore>();

        public string? ProfileImagePath { get; set; }

    }
}
