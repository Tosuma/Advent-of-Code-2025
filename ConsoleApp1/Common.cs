using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode;

internal static class Common
{
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
