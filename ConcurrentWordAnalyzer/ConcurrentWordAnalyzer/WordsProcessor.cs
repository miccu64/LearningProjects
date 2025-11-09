using System.Collections.Concurrent;
using ConcurrentWordAnalyzer.Models;

namespace ConcurrentWordAnalyzer;

public static class WordsProcessor
{
    public static void SingleThread(IList<string> words)
    {
        ResultBuilder resultBuilder = new();

        SingleThreadWordsProcessor processor = new();
        processor.Process(words);

        resultBuilder.AppendResults(processor);
        resultBuilder.PrintResults();
    }

    public static void ParallelForEach(IList<string> words)
    {
        ResultBuilder resultBuilder = new();

        Parallel.ForEach(
            Partitioner.Create(0, words.Count),
            () => new SingleThreadWordsProcessor(),
            (range, _, processor) =>
            {
                List<string> wordsFromRange = words
                    .Skip(range.Item1)
                    .Take(range.Item2 - range.Item1)
                    .ToList();

                processor.Process(wordsFromRange);
                return processor;
            },
            processor => { resultBuilder.AppendResults(processor); }
        );

        resultBuilder.PrintResults();
    }

    public static void ParallelFor(IList<string> words)
    {
        int count = words.Count;

        ResultBuilder resultBuilder = new();

        Parallel.For(
            0,
            count,
            () => new SingleThreadWordsProcessor(),
            (i, _, processor) =>
            {
                processor.Process(words[i]);
                return processor;
            },
            processor => { resultBuilder.AppendResults(processor); }
        );

        resultBuilder.PrintResults();
    }

    public static void PLinq(IList<string> words)
    {
        ResultBuilder resultBuilder = new();

        words.AsParallel()
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .Aggregate(
                () => new SingleThreadWordsProcessor(),
                (processor, word) =>
                {
                    processor.Process(word);
                    return processor;
                },
                (processor, func) =>
                {
                    resultBuilder.AppendResults(processor);
                    return func;
                },
                processor => processor
            );

        resultBuilder.PrintResults();
    }
}