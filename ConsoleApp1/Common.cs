using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode;

internal static class Common
{
    public static void Result<T>(string msg, T guess, T correct) where T : IComparable<T>
        => Console.WriteLine($"{msg,-40}: {guess,-20} --> Is correct ? {guess.Equals(correct)}");
    public static IEnumerable<string> ReadLines(this TextReader reader)
    {
        while (reader.ReadLine() is string line)
            yield return line;
    }

    public static ulong Sum(this IEnumerable<ulong> source)
    {
        ulong sum = 0;
        foreach (ulong value in source)
        {
            sum += value;
        }

        return sum;
    }
}
