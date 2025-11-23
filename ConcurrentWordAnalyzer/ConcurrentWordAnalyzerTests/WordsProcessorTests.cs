using ConcurrentWordAnalyzer;
using ConcurrentWordAnalyzer.Utils;

namespace ConcurrentWordAnalyzerTests;

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
        string result5 = WordsProcessor.ManualTasks(words);

        // Assert
        List<string> results = [result2, result3, result4, result5];

        foreach (string r in results)
            Assert.Equal(result1, r);
    }

    [Fact]
    public void AllParallelMethodsShouldThrowOnCancelledToken()
    {
        // Arrange
        List<string> words = DataGeneratorUtils.GenerateWords(10).ToList();

        using CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;
        cts.Cancel();

        List<Action> actions =
        [
            () => WordsProcessor.ParallelFor(words, token),
            () => WordsProcessor.ParallelForEach(words, token),
            () => WordsProcessor.PLinq(words, token),
            () => WordsProcessor.ManualTasks(words, token)
        ];

        // Act / Assert
        foreach (Action action in actions)
        {
            Exception ex = Assert.ThrowsAny<Exception>(action);

            Assert.True((ex is AggregateException ? ex.InnerException : ex) is OperationCanceledException);
        }
    }
}