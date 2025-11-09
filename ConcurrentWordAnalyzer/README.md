# Parallel Word Processor

This project demonstrates different approaches to processing a large collection of words using C# and .NET parallelism.  
It generates **1,000,000 random words** and processes them using four strategies:

- **SingleThread** – baseline sequential implementation
- **ParallelFor** – manual parallelization using Parallel.For
- **ParallelForEach** – range-based partitioning using Parallel.ForEach and Partitioner.Create
- **PLinq** – parallel LINQ query with per-thread local accumulators.

Each method counts word occurrences, aggregates word-length statistics, and measures execution time.
