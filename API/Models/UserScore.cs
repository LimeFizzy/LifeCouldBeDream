namespace API.Models
{
    public class UserScore : IComparable<UserScore>
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public int Score { get; set; }
        public string GameType { get; set; } = "";
        public required string GameDate { get; set; }  // why it was not requared before?

        public int CompareTo(UserScore? other)
        {
            if (other == null) return 1;

            return other.Score.CompareTo(this.Score);
        }
    }
}
