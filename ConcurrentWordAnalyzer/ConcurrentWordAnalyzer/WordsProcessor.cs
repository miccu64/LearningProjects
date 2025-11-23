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

    public static string ParallelForEach(IList<string> words, CancellationToken? token = null)
    {
        ResultBuilder resultBuilder = new();

        ParallelOptions options = new()
        {
            CancellationToken = token ?? CancellationToken.None,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        Parallel.ForEach(
            Partitioner.Create(0, words.Count),
            options,
            () => new SingleThreadWordsProcessor(),
            (range, _, processor) => processor.Process(words, range.Item1, range.Item2),
            processor => { resultBuilder.AppendResults(processor); }
        );

        resultBuilder.PrintResults();
        return resultBuilder.ToString();
    }

    public static string ParallelFor(IList<string> words, CancellationToken? token = null)
    {
        ResultBuilder resultBuilder = new();

        ParallelOptions options = new()
        {
            CancellationToken = token ?? CancellationToken.None,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        Parallel.For(
            0,
            words.Count,
            options,
            () => new SingleThreadWordsProcessor(),
            (i, _, processor) => processor.Process(words[i]),
            processor => { resultBuilder.AppendResults(processor); }
        );

        resultBuilder.PrintResults();
        return resultBuilder.ToString();
    }

    public static string PLinq(IList<string> words, CancellationToken? token = null)
    {
        ResultBuilder resultBuilder = new();

        OrderablePartitioner<Tuple<int, int>> rangePartitioner = Partitioner.Create(0, words.Count);

        rangePartitioner.AsParallel()
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .WithCancellation(token ?? CancellationToken.None)
            .ForAll(range =>
            {
                SingleThreadWordsProcessor processor = new SingleThreadWordsProcessor()
                    .Process(words, range.Item1, range.Item2);

                resultBuilder.AppendResults(processor);
            });

        resultBuilder.PrintResults();
        return resultBuilder.ToString();
    }

    public static string ManualTasks(IList<string> words, CancellationToken? token = null)
    {
        ResultBuilder resultBuilder = new();
        List<Task> tasks = [];

        int tasksCount = Environment.ProcessorCount;

        int chunkSize = words.Count / tasksCount;
        int remainder = words.Count % tasksCount;

        int start = 0;

        for (int i = 0; i < tasksCount; i++)
        {
            int localStart = start;
            int localCount = chunkSize + (i < remainder ? 1 : 0);

            start += localCount;

            Task task = Task.Run(() =>
            {
                IEnumerable<string> localWords = words
                    .Skip(localStart)
                    .Take(localCount);

                SingleThreadWordsProcessor processor = new SingleThreadWordsProcessor()
                    .Process(localWords.ToList());

                resultBuilder.AppendResults(processor);
            }, token ?? CancellationToken.None);

            tasks.Add(task);
        }

        Task.WaitAll(tasks);

        resultBuilder.PrintResults();
        return resultBuilder.ToString();
    }
}