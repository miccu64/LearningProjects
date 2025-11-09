using ConcurrentWordAnalyzer;
using ConcurrentWordAnalyzer.Utils;

namespace ConcurrentWordAnalyzerTests.UtilsTests;

public class WordsProcessorTests
{
    [Fact]
    public void AllMethodsShouldReturnSameResults()
    {
        // Arrange
        const int wordsCount = 10;
        List<string> words = DataGeneratorUtils.GenerateWords(wordsCount).ToList();

        // Act
        string result1 = WordsProcessor.SingleThread(words);
        string result2 = WordsProcessor.ParallelFor(words);
        string result3 = WordsProcessor.ParallelForEach(words);
        string result4 = WordsProcessor.PLinq(words);

        // Assert
        Assert.Equal(result1, result2);
        Assert.Equal(result1, result3);
        Assert.Equal(result1, result4);
    }
}