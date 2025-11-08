namespace ConcurrentWordAnalyzer;

public class WordsProcessor
{
    public void SingleThread(IEnumerable<string> words)
    {
        SingleThreadWordsProcessor processor = new();
        processor.Process(words);
    }
}