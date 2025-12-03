using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;

namespace AdventOfCode;

internal class Day02
{
    record Range(ulong From, ulong To);

    public static void Run(TextReader reader)
    {
        HashSet<ulong> seen = [];
        var input = reader.ReadLine() ?? "";

        var ranges = GetRanges(input);

        var invalids = ranges
            .SelectMany(GetNumbers)
            .Where(num => num.ToString().Length % 2 == 0)
            .Where(value =>
            {
                var str = value.ToString();
                int middle = str.Length / 2;

                return str[0..middle] == str[middle..];
            })
            .ToArray();


        ulong invalidSum = 0;
        for (int j = 0; j < invalids.Length; j++)
        {
            invalidSum += invalids[j];
        }

        Console.WriteLine($"Sum of invalid IDs : {invalidSum} --> Is correct ? {invalidSum == 28844599675}");
        //Console.WriteLine($"The number of : {zeroPasses} --> Is correct ? {zeroPasses == 6106}");
    }

    private static IEnumerable<Range> GetRanges(string input) => input
            .Split(',') // split ranges
            .Select(strRange => strRange.Split('-')) // split range pairs
            .Select(strs =>
            {
                var start = ulong.Parse(strs[0]);
                var end = ulong.Parse(strs[1]);
                return new Range(start, end);
            });

    private static IEnumerable<ulong> GetNumbers(Range range)
    {
        for (ulong num = range.From; num <= range.To; num++)
            yield return num;
    }
}
