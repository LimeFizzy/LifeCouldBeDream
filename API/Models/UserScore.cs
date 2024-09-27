using System;

namespace API.Models
{
    public class UserScore
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public int Score { get; set; }
        public DateTime GameDate { get; set; } = DateTime.Now;
    }
}
