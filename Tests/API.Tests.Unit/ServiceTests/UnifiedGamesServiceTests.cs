namespace API.Tests.Unit.ServiceTests
{
    public class UnifiedGamesServiceTests
    {
        private readonly UnifiedGamesService<int> _unifiedGamesServiceLN;
        private readonly UnifiedGamesService<Square> _unifiedGamesServiceSequence;
        private readonly ILogger<UnifiedGamesService<int>> _loggerLN;
        private readonly ILogger<UnifiedGamesService<Square>> _loggerSequence;

        public UnifiedGamesServiceTests()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });

            _loggerLN = loggerFactory.CreateLogger<UnifiedGamesService<int>>();
            _loggerSequence = loggerFactory.CreateLogger<UnifiedGamesService<Square>>();

            _unifiedGamesServiceLN = new UnifiedGamesService<int>(_loggerLN);
            _unifiedGamesServiceSequence = new UnifiedGamesService<Square>(_loggerSequence);
        }

        // Long Number Tests
        [Fact]
        public void GenerateSequenceLN_ThrowsExceptionForInvalidLevel()
        {
            // Arrange
            int invalidLevel = 0;

            // Act
            Action act = () => _unifiedGamesServiceLN.GenerateSequence(invalidLevel);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Level must be greater than zero.*");
        }

        [Fact]
        public void CalculateScoreLN_ThrowsExceptionForInvalidLevel()
        {
            // Arrange
            int invalidLevel = 0;

            // Act
            Action act = () => _unifiedGamesServiceLN.CalculateScore(invalidLevel);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Level must be greater than or equal to 1.*");
        }

        [Fact]
        public void GenerateSequenceLN_HandlesRandomness()
        {
            // Arrange
            int level = 5;

            // Act
            var sequence1 = _unifiedGamesServiceLN.GenerateSequence(level);
            var sequence2 = _unifiedGamesServiceLN.GenerateSequence(level);

            // Assert
            sequence1.Should().NotEqual(sequence2); // Different sequences due to randomness
        }

        // Sequence Tests
        [Fact]
        public void GenerateSequenceSq_ThrowsExceptionForInvalidLevel()
        {
            // Arrange
            int invalidLevel = -1;

            // Act
            Action act = () => _unifiedGamesServiceSequence.GenerateSequence(invalidLevel);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Level must be greater than zero.*");
        }

        [Fact]
        public void CalculateScoreSq_ThrowsExceptionForInvalidLevel()
        {
            // Arrange
            int invalidLevel = -1;

            // Act
            Action act = () => _unifiedGamesServiceSequence.CalculateScore(invalidLevel);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Level must be greater than or equal to 1.*");
        }

        [Fact]
        public void GenerateSequenceSq_HandlesRandomness()
        {
            // Arrange
            int level = 5;

            // Act
            var sequence1 = _unifiedGamesServiceSequence.GenerateSequence(level);
            var sequence2 = _unifiedGamesServiceSequence.GenerateSequence(level);

            // Assert
            sequence1.Should().NotEqual(sequence2); // Different sequences due to randomness
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(3, 2)]
        [InlineData(5, 6)] // Covers logic for level > 2
        [InlineData(6, 10)] // Extended coverage
        public void CalculateScoreSq_HandlesAllLogicPaths(int level, int expectedScore)
        {
            // Act
            var result = _unifiedGamesServiceSequence.CalculateScore(level);

            // Assert
            result.Should().Be(expectedScore);
        }

        // Additional Edge Case Tests
        [Fact]
        public void GenerateSequenceLN_HandlesMaxLevel()
        {
            // Arrange
            int maxLevel = 1000;

            // Act
            var sequence = _unifiedGamesServiceLN.GenerateSequence(maxLevel);

            // Assert
            sequence.Should().HaveCount(maxLevel);
        }

        [Fact]
        public void GenerateSequenceSq_HandlesMaxLevel()
        {
            // Arrange
            int maxLevel = 1000;

            // Act
            var sequence = _unifiedGamesServiceSequence.GenerateSequence(maxLevel);

            // Assert
            sequence.Should().HaveCount(maxLevel);
            sequence.Should().AllBeOfType<Square>();
        }

        [Fact]
        public void CalculateScoreLN_HandlesMaxLevel()
        {
            // Arrange
            int maxLevel = 1000;

            // Act
            var result = _unifiedGamesServiceLN.CalculateScore(maxLevel);

            // Assert
            result.Should().Be(maxLevel - 1);
        }

        [Fact]
        public void CalculateScoreSq_HandlesMaxLevel()
        {
            // Arrange
            int maxLevel = 1000;

            // Act
            var result = _unifiedGamesServiceSequence.CalculateScore(maxLevel);

            // Assert
            result.Should().Be((maxLevel - 1) * (maxLevel - 2) / 2);
        }
    }
}
