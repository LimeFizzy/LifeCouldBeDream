namespace API.Models
{
    public class UserScore : IComparable<UserScore>
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public int Score { get; set; }
        public string GameType { get; set; } = "";
        public string GameDate { get; set; }

        public int CompareTo(UserScore? other)
        {
            if (other == null) return 1;

            return other.Score.CompareTo(this.Score);
        }
    }
}
