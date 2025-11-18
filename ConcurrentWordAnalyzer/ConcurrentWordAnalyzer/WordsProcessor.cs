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

    public static string ManualTasks(IList<string> words, int tasksCount)
    {
        if (tasksCount <= 0)
            throw new ArgumentException("Tasks count must be greater than 0.");
        if (words.Count < tasksCount)
            throw new ArgumentException($"The number of tasks must be at least {words.Count} tasks.");

        ResultBuilder resultBuilder = new();
        List<Task> tasks = [];
        
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
            });

            tasks.Add(task);
        }

        Task.WaitAll(tasks);

        resultBuilder.PrintResults();
        return resultBuilder.ToString();
    }
}