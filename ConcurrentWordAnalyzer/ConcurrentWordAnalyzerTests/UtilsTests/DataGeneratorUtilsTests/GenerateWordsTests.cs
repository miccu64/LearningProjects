using ConcurrentWordAnalyzer.Utils;

namespace ConcurrentWordAnalyzerTests.UtilsTests.DataGeneratorUtilsTests;

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

    [Fact]
    public void ShouldWorkConcurrently()
    {
        // Arrange
        const int count = 1000;
        const int range = 10;

        // Act
        List<string> words = Enumerable.Range(0, range)
            .AsParallel()
            .SelectMany(_ => DataGeneratorUtils.GenerateWords(count))
            .ToList();
        
        // Assert
        bool allHaveSomeChars = words.All(word => word.Length > 0);
        Assert.True(allHaveSomeChars);

        bool haveSomeDistinctValues = words.Distinct().Count() > range;
        Assert.True(haveSomeDistinctValues);
    }
}