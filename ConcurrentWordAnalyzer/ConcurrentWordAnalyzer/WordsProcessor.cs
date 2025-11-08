using System.Collections.Concurrent;

namespace ConcurrentWordAnalyzer;

public class WordsProcessor
{
    public void SingleThread(ICollection<string> words)
    {
        SingleThreadWordsProcessor processor = new();
        processor.Process(words);
    }

    public void ParallelFor(ICollection<string> words)
    {
        Parallel.ForEach(
            Partitioner.Create(0, words.Count),
            () => new SingleThreadWordsProcessor(),
            (range, _,processor) =>
            {
                List<string> wordsFromRange = words
                    .Skip(range.Item1)
                    .Take(range.Item2)
                    .ToList();

                processor.Process(wordsFromRange);
                
                return processor;
            },
            localData =>
            {
                
            }
        );
    }
}