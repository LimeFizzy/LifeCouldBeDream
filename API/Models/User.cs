using System;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class User : IComparable<User>
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }

        [JsonIgnore]
        public ICollection<UserScore> Scores { get; set; } = new List<UserScore>();

        public string? ProfileImagePath { get; set; }

        public int CompareTo(User? other)
        {
            if (other == null)
                return 1;

            return string.Compare(PasswordHash, other.PasswordHash, StringComparison.Ordinal);
        }
    }
}
