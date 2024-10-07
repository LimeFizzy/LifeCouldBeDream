namespace API.Models
{
    public record ScoreSubmission
    {
        public string Username { get; set; } = "guest";
        public string GameType { get; set; } = "";
        public int[] GuessedSequence { get; set; } = [];
        public int[] CorrectSequence { get; set; } = [];
        public int Level { get; set; }
    }
}
