using System.Text.Json.Serialization;

namespace API.Models;

public class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }

    [JsonIgnore]
    public ICollection<UserScore> Scores { get; set; } = new List<UserScore>();
}