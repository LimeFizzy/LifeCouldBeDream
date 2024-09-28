namespace API.Models
{
    public record ScoreSubmission
    {
        public string Username { get; set; } = "guest";
        public int[] GuessedSequence { get; set; } = [];
        public int[] CorrectSequence { get; set; } = [];
        public int Level { get; set; }
    }
}
