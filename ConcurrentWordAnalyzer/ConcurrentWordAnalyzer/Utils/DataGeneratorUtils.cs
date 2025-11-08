namespace ConcurrentWordAnalyzer.Utils;

public static class DataGeneratorUtils
{
    public static IEnumerable<string> GenerateWords(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        for (int i = 0; i < count; i++)
            yield return RandomString();
    }

    private static string RandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int length = Random.Shared.Next(1, 60);

        return new string(
            Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Shared.Next(s.Length)])
                .ToArray()
        );
    }
}