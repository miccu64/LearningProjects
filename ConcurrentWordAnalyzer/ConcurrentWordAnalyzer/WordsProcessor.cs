using System.Collections.Concurrent;
using ConcurrentWordAnalyzer.Models;

namespace ConcurrentWordAnalyzer;

public static class WordsProcessor
{
    public static string SingleThread(IList<string> words)
    {
        ResultBuilder resultBuilder = new();

        SingleThreadWordsProcessor processor = new SingleThreadWordsProcessor()
            .Process(words);

        resultBuilder.AppendResults(processor);

        resultBuilder.PrintResults();
        return resultBuilder.ToString();
    }

    public static string ParallelForEach(IList<string> words)
    {
        ResultBuilder resultBuilder = new();

        Parallel.ForEach(
            Partitioner.Create(0, words.Count),
            () => new SingleThreadWordsProcessor(),
            (range, _, processor) => processor.Process(words, range.Item1, range.Item2),
            processor => { resultBuilder.AppendResults(processor); }
        );

        resultBuilder.PrintResults();
        return resultBuilder.ToString();
    }

    public static string ParallelFor(IList<string> words)
    {
        ResultBuilder resultBuilder = new();

        Parallel.For(
            0,
            words.Count,
            () => new SingleThreadWordsProcessor(),
            (i, _, processor) => processor.Process(words[i]),
            processor => { resultBuilder.AppendResults(processor); }
        );

        resultBuilder.PrintResults();
        return resultBuilder.ToString();
    }

    public static string PLinq(IList<string> words)
    {
        ResultBuilder resultBuilder = new();

        OrderablePartitioner<Tuple<int, int>> rangePartitioner = Partitioner.Create(0, words.Count);

        rangePartitioner.AsParallel()
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .ForAll(range =>
            {
                SingleThreadWordsProcessor processor = new SingleThreadWordsProcessor()
                    .Process(words, range.Item1, range.Item2);

                resultBuilder.AppendResults(processor);
            });

        resultBuilder.PrintResults();
        return resultBuilder.ToString();
    }
}