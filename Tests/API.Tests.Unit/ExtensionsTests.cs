using API.Extensions;

namespace API.Tests.Unit.ExtensionTests
{
    public class ArrayExtensionsTests
    {
        [Fact]
        public void IsValidSequence_ReturnsTrue_WhenValidSequence()
        {
            // Arrange
            var sequence = new int[] { 1, 2, 3, 4, 5 };

            // Act
            var result = sequence.IsValidSequence(5);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidSequence_ReturnsFalse_WhenSequenceLengthIsIncorrect()
        {
            // Arrange
            var sequence = new int[] { 1, 2, 3, 4, 5 };

            // Act
            var result = sequence.IsValidSequence(6); // Expected length is 6, but the array has 5 elements

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidSequence_ReturnsFalse_WhenSequenceHasInvalidValue()
        {
            // Arrange
            var sequence = new int[] { 1, 2, 3, -1, 5 };  // Contains -1, which is invalid

            // Act
            var result = sequence.IsValidSequence(5);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidSequence_ReturnsFalse_WhenSequenceHasOutOfRangeValue()
        {
            // Arrange
            var sequence = new int[] { 1, 10, 3, 4, 5 };  // Contains 10, which is invalid

            // Act
            var result = sequence.IsValidSequence(5);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidSequence_ReturnsFalse_WhenSequenceHasNegativeValue()
        {
            // Arrange
            var sequence = new int[] { -1, 2, 3, 4, 5 };  // Contains -1, which is invalid

            // Act
            var result = sequence.IsValidSequence(5);

            // Assert
            result.Should().BeFalse();
        }
    }

    public class SequenseGeneratorTests
    {
        private readonly SequenseGenerator _generator;

        public SequenseGeneratorTests()
        {
            _generator = new SequenseGenerator();
        }

        [Fact]
        public void GenerateSequence_ReturnsSequenceOfCorrectLength()
        {
            // Arrange
            int level = 5;

            // Act
            var result = _generator.GenerateSequence(level, random => random.Next(0, 10));

            // Assert
            result.Should().HaveCount(level);  // The generated sequence should have exactly 'level' number of elements.
        }

        [Fact]
        public void GenerateSequence_ReturnsSequenceWithCorrectItemType()
        {
            // Arrange
            int level = 3;

            // Act
            var result = _generator.GenerateSequence(level, random => random.Next(0, 10));

            // Assert
            result.Should().AllBeOfType<int>();  // All items in the generated sequence should be of type int.
        }

        [Fact]
        public void GenerateSequence_ReturnsValuesWithinRange()
        {
            // Arrange
            int level = 10;

            // Act
            var result = _generator.GenerateSequence(level, random => random.Next(0, 10));

            // Assert
            result.Should().OnlyContain(num => num >= 0 && num <= 9);  // All values should be between 0 and 9.
        }

        [Fact]
        public void GenerateSequence_ReturnsRandomValues()
        {
            // Arrange
            int level = 5;

            // Act
            var result1 = _generator.GenerateSequence(level, random => random.Next(0, 10));
            var result2 = _generator.GenerateSequence(level, random => random.Next(0, 10));

            // Assert
            result1.Should().NotEqual(result2);  // The two generated sequences should not be the same because of randomness.
        }

        [Fact]
        public void GenerateSequence_ReturnsEmptyArray_WhenLevelIsZero()
        {
            // Arrange
            int level = 0;

            // Act
            var result = _generator.GenerateSequence(level, random => random.Next(0, 10));

            // Assert
            result.Should().BeEmpty();  // The generated sequence should be an empty array when level is 0.
        }
    }
}
