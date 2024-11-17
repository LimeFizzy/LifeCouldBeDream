namespace API.Tests.Unit.ServiceTests
{
    public class UnifiedGamesServiceTests
    {
        private readonly UnifiedGamesService<int> _unifiedGamesServiceLN;
        private readonly UnifiedGamesService<Square> _unifiedGamesServiceSequence;
        private readonly LongNumberController _longNumberController;
        private readonly SequenceController _sequenceController;
        private readonly ILogger<LongNumberController> _loggerLNController;
        private readonly ILogger<SequenceController> _loggerSequenceController;
        private readonly ILogger<UnifiedGamesService<int>> _loggerLN;
        private readonly ILogger<UnifiedGamesService<Square>> _loggerSequence;

        public UnifiedGamesServiceTests()
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // or .WriteTo.File("log.txt")
                .CreateLogger();

            // Wrap Serilog in Microsoft.Extensions.Logging.ILogger
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
            _loggerLN = loggerFactory.CreateLogger<UnifiedGamesService<int>>();
            _loggerLNController = loggerFactory.CreateLogger<LongNumberController>();

            _loggerSequence = loggerFactory.CreateLogger<UnifiedGamesService<Square>>();
            _loggerSequenceController = loggerFactory.CreateLogger<SequenceController>();

            _unifiedGamesServiceLN = new UnifiedGamesService<int>(_loggerLN);
            _unifiedGamesServiceSequence = new UnifiedGamesService<Square>(_loggerSequence);

            _longNumberController = new LongNumberController(_loggerLNController, _unifiedGamesServiceLN);
            _sequenceController = new SequenceController(_loggerSequenceController, _unifiedGamesServiceSequence);
        }

        [Fact]
        public void GenerateSequenceLN_ReturnsArrayOfCorrectLength()
        {
            // Arrange
            int level = 5;

            // Act
            var result = _unifiedGamesServiceLN.GenerateSequence(_longNumberController, level);

            // Assert
            result.Should().HaveCount(level);
        }
        [Fact]
        public void GenerateSequenceLN_ReturnsNumbersInExpectedRange()
        {
            // Arrange
            int level = 10;

            // Act
            var result = _unifiedGamesServiceLN.GenerateSequence(_longNumberController, level);

            // Assert
            result.Should().OnlyContain(x => x >= 0 && x < 10);
        }

        [Theory]
        [InlineData(1, 0)]  // level = 1 should return score of 0
        [InlineData(2, 1)]  // level = 2 should return score of 1
        [InlineData(10, 9)] // level = 10 should return score of 9
        public void CalculateScoreLN_ReturnsExpectedScore(int level, int expectedScore)
        {
            // Act
            var result = _unifiedGamesServiceLN.CalculateScore(GameTypes.LONG_NUMBER, level);

            // Assert
            result.Should().Be(expectedScore);
        }


        [Fact]
        public void GenerateSequenceSq_ReturnsCorrectLength()
        {
            // Arrange
            int level = 5;

            // Act
            var sequence = _unifiedGamesServiceSequence.GenerateSequence(_sequenceController, level);

            // Assert
            sequence.Should().HaveCount(level);
        }
        [Fact]
        public void GenerateSequenceSq_ReturnsSquaresWithCorrectValues()
        {
            // Arrange
            int level = 5;

            // Act
            var sequence = _unifiedGamesServiceSequence.GenerateSequence(_sequenceController, level);

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
        [InlineData(3, 2)]
        [InlineData(4, 3)]
        public void CalculateScoreSq_ReturnsExpectedScore(int level, int expectedScore)
        {
            // Act
            var score = _unifiedGamesServiceSequence.CalculateScore(GameTypes.SEQUENCE, level);

            // Assert
            score.Should().Be(expectedScore);
        }

        [Fact]
        public void CalculateScoreSq_ReturnsZeroForLevelOne()
        {
            // Arrange
            int level = 1;

            // Act
            var score = _unifiedGamesServiceSequence.CalculateScore(GameTypes.SEQUENCE, level);

            // Assert
            score.Should().Be(0);
        }
    }
}
