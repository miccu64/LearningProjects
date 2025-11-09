using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace ConcurrentWordAnalyzer.Models;

public class ResultBuilder
{
    private readonly ConcurrentDictionary<string, int> _wordCounters = new();
    private readonly ConcurrentDictionary<int, int> _wordLengthCounters = new();
    private int _shortestWordLength = int.MaxValue;
    private int _longestWordLength;

    private readonly Lock _lockObject = new();
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public void AppendResults(SingleThreadWordsProcessor processor)
    {
        foreach (KeyValuePair<string, int> wordCounter in processor.WordCounters)
        {
            _wordCounters.AddOrUpdate(
                wordCounter.Key,
                wordCounter.Value,
                (_, value) => value + wordCounter.Value
            );
        }

        foreach (KeyValuePair<int, int> wordCounter in processor.WordLengthCounters)
        {
            _wordLengthCounters.AddOrUpdate(
                wordCounter.Key,
                wordCounter.Value,
                (_, value) => value + wordCounter.Value
            );
        }

        lock (_lockObject)
        {
            if (_shortestWordLength > processor.ShortestWordLength)
                _shortestWordLength = processor.ShortestWordLength;

            if (_longestWordLength < processor.LongestWordLength)
                _longestWordLength = processor.LongestWordLength;
        }
    }

    public void PrintResults()
    {
        _stopwatch.Stop();

        List<string> mostFrequentWords = _wordCounters
            .OrderByDescending(kvp => kvp.Value)
            .Take(5)
            .Select(kvp => kvp.Key)
            .ToList();

        List<string> mostFrequentWordLengths = _wordLengthCounters
            .OrderByDescending(kvp => kvp.Value)
            .Take(5)
            .Select(kvp => $"length {kvp.Key} (count {kvp.Value})")
            .ToList();

        StringBuilder sb = new StringBuilder()
            .AppendLine()
            .AppendLine($"Elapsed time: {_stopwatch.ElapsedMilliseconds} ms")
            .AppendLine($"Shortest word length: {_shortestWordLength}")
            .AppendLine($"Longest word length: {_longestWordLength}")
            .AppendLine($"Most frequent words: {string.Join(", ", mostFrequentWords)}")
            .AppendLine($"Most frequent word lengths: {string.Join(", ", mostFrequentWordLengths)}")
            .AppendLine();

        Console.WriteLine(sb.ToString());
    }
}