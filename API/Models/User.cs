using System;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class User : IComparable<User>
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }

        public bool IsAdmin { get; set; } = false;

        [JsonIgnore]
        public ICollection<UserScore> Scores { get; set; } = new List<UserScore>();

        public string? ProfileImagePath { get; set; }

        public int CompareTo(User? other)    // 10. Implement at least one of the standard .NET interfaces
        {
            if (other == null)
                return 1;

            return string.Compare(PasswordHash, other.PasswordHash, StringComparison.Ordinal);
        }
    }
}
