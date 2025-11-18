using ConcurrentWordAnalyzer;
using ConcurrentWordAnalyzer.Utils;

const int wordsCount = 1_000_000;
List<string> words = DataGeneratorUtils.GenerateWords(wordsCount).ToList();

WordsProcessor.SingleThread(words);
WordsProcessor.ParallelFor(words);
WordsProcessor.ParallelForEach(words);
WordsProcessor.PLinq(words);
WordsProcessor.ManualTasks(words, 16);
