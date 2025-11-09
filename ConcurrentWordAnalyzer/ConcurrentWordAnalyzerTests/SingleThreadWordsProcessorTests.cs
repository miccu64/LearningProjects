using ConcurrentWordAnalyzer;

namespace ConcurrentWordAnalyzerTests;

public class SingleThreadWordsProcessorTests
{
    [Fact]
    public void ShouldProcessSingleWord()
    {
        // Arrange
        SingleThreadWordsProcessor processor = new();
        const string word = "hello";

        // Act
        processor.Process(word);

        // Assert        
        Assert.Equal(1, processor.WordCounters[word]);
        Assert.Equal(1, processor.WordLengthCounters[word.Length]);
        Assert.Equal(word.Length, processor.ShortestWordLength);
        Assert.Equal(word.Length, processor.LongestWordLength);
    }

    [Fact]
    public void ShouldProcessMultipleWords()
    {
        // Arrange
        SingleThreadWordsProcessor processor = new();
        List<string> words = ["hi", "hello", "hi"];

        // Act
        processor.Process(words);

        // Assert
        Assert.Equal(2, processor.WordCounters["hi"]);
        Assert.Equal(1, processor.WordCounters["hello"]);

        Assert.Equal(2, processor.WordLengthCounters[2]);
        Assert.Equal(1, processor.WordLengthCounters[5]);

        Assert.Equal(2, processor.ShortestWordLength);
        Assert.Equal(5, processor.LongestWordLength);
    }
}