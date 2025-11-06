using ConcurrentWordAnalyzer.Utils;

namespace ConcurrentWordAnalyzerTests;

public class GenerateWordsTests
{
    [Fact]
    public void ShouldReturnSingleWord()
    {
        // Arrange
        const int count = 1;

        // Act
        List<string> words = DataGeneratorUtils.GenerateWords(count).ToList();

        // Assert
        Assert.Single(words);
    }

    [Fact]
    public void ShouldReturnNotEmptyWord()
    {
        // Arrange
        const int count = 1;

        // Act
        List<string> words = DataGeneratorUtils.GenerateWords(count).ToList();

        // Assert
        Assert.True(words[0].Length > 0);
    }

    [Fact]
    public void ShouldReturnManyWords()
    {
        // Arrange
        const int count = 5;

        // Act
        List<string> words = DataGeneratorUtils.GenerateWords(count).ToList();

        // Assert
        Assert.Equal(count, words.Count);
    }
}