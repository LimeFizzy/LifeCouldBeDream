namespace API.Tests.Unit.ServiceTests
{
    public class UnifiedGamesServiceTests
    {
        private readonly UnifiedGamesService<int> _unifiedGamesServiceLN;
        private readonly UnifiedGamesService<Square> _unifiedGamesServiceSequence;
        private readonly UnifiedGamesService<SquareChimp> _unifiedGamesServiceSquareChimp;
        private readonly ILogger<UnifiedGamesService<int>> _loggerLN;
        private readonly ILogger<UnifiedGamesService<Square>> _loggerSequence;
        private readonly ILogger<UnifiedGamesService<SquareChimp>> _loggerSquareChimp;

        

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
            _loggerSquareChimp = loggerFactory.CreateLogger<UnifiedGamesService<SquareChimp>>();
            
            _unifiedGamesServiceLN = new UnifiedGamesService<int>(_loggerLN);
            _unifiedGamesServiceSequence = new UnifiedGamesService<Square>(_loggerSequence);
            _unifiedGamesServiceSquareChimp = new UnifiedGamesService<SquareChimp>(_loggerSquareChimp);
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

        [Fact]
        public void GenerateSequenceSquareChimp_ThrowsExceptionForInvalidLevel()
        {
            // Arrange
            int invalidLevel = 0; // zero or negative will cause error

            // Act
            Action act = () => _unifiedGamesServiceSquareChimp.GenerateSequence(invalidLevel);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Level must be greater than zero.*");
        }

        [Fact]
        public void GenerateSequenceSquareChimp_ReturnsUniqueCoordinates()
        {
            // Arrange
            int level = 10;

            // Act
            var sequence = _unifiedGamesServiceSquareChimp.GenerateSequence(level);
            var coords = sequence.Select(s => (s.X, s.Y)).ToList();

            // Assert
            sequence.Should().HaveCount(level);
            coords.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void CalculateScoreSquareChimp_ThrowsExceptionForInvalidLevel()
        {
            int invalidLevel = 0;
            Action act = () => _unifiedGamesServiceSquareChimp.CalculateScore(invalidLevel);
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Level must be greater than or equal to 1.*");
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 3)]
        [InlineData(3, 7)]
        [InlineData(10, 63)]
        public void CalculateScoreSquareChimp_HandlesLogicPaths(int level, int expectedScore)
        {
            var result = _unifiedGamesServiceSquareChimp.CalculateScore(level);
            result.Should().Be(expectedScore);
        }

        [Fact]
        public void GenerateSequenceSquareChimp_HandlesMaxLevel()
        {
            // Arrange
            int maxLevel = 40; // Because there are only 40 possible unique coordinates (8x5=40)

            // Act
            var sequence = _unifiedGamesServiceSquareChimp.GenerateSequence(maxLevel);

            // Assert
            sequence.Should().HaveCount(maxLevel);
            sequence.Should().AllBeOfType<SquareChimp>();
            
            // Check uniqueness and coverage
            var coords = sequence.Select(s => (s.X, s.Y)).ToList();
            coords.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void CalculateScoreSquareChimp_HandlesLargeLevel()
        {
             // Arrange
            int level = 100;
            var expected = 5148;

            // Act
            var result = _unifiedGamesServiceSquareChimp.CalculateScore(level);

            // Assert
            result.Should().Be(expected);
        }
    }
}
