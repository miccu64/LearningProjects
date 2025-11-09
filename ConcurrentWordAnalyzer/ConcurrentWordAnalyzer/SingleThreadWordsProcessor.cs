namespace ConcurrentWordAnalyzer;

public class SingleThreadWordsProcessor
{
    public IDictionary<string, int> WordCounters { get; } = new Dictionary<string, int>();
    public IDictionary<int, int> WordLengthCounters { get; } = new Dictionary<int, int>();
    public int ShortestWordLength { get; private set; } = int.MaxValue;
    public int LongestWordLength { get; private set; }

    public SingleThreadWordsProcessor Process(IList<string> words)
    {
        foreach (string word in words)
            Process(word);

        return this;
    }

    public SingleThreadWordsProcessor Process(IList<string> words, int start, int end)
    {
        for (int i = start; i < end; i++)
            Process(words[i]);

        return this;
    }

    public SingleThreadWordsProcessor Process(string word)
    {
        CountWord(word);
        ProcessLength(word);

        return this;
    }

    private void CountWord(string word)
    {
        WordCounters.TryGetValue(word, out int counter);
        WordCounters[word] = counter + 1;
    }

    private void ProcessLength(string word)
    {
        int length = word.Length;

        if (ShortestWordLength > length)
            ShortestWordLength = length;

        if (LongestWordLength < length)
            LongestWordLength = length;

        WordLengthCounters.TryGetValue(length, out int counter);
        WordLengthCounters[length] = counter + 1;
    }
}