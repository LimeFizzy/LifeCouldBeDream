namespace API.Tests.Unit.ServiceTests
{
    public class SequenceServiceTests
    {
        private readonly SequenceService _sequenceService;

        public SequenceServiceTests()
        {
            _sequenceService = new SequenceService();
        }

        [Fact]
        public void GenerateSequence_ReturnsCorrectLength()
        {
            // Arrange
            int level = 5;

            // Act
            var sequence = _sequenceService.GenerateSequence(level);

            // Assert
            sequence.Should().HaveCount(level);
        }

        [Fact]
        public void GenerateSequence_ReturnsSquaresWithCorrectValues()
        {
            // Arrange
            int level = 5;

            // Act
            var sequence = _sequenceService.GenerateSequence(level);

            // Assert
            foreach (var square in sequence)
            {
                square.Id.Should().BeInRange(1, 9);  // Corrected from Value to Id
                square.IsActive.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 3)]
        [InlineData(4, 6)]
        public void CalculateScore_ReturnsExpectedScore(int level, int expectedScore)
        {
            // Act
            var score = _sequenceService.CalculateScore(level);

            // Assert
            score.Should().Be(expectedScore);
        }

        [Fact]
        public void CalculateScore_ReturnsZeroForLevelOne()
        {
            // Arrange
            int level = 1;

            // Act
            var score = _sequenceService.CalculateScore(level);

            // Assert
            score.Should().Be(0);
        }
    }
}
