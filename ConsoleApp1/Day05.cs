using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode;

internal static class Day05
{
    record Range(ulong Start, ulong End);
    public static void Run(TextReader reader)
    {
        var (ranges, ids) = reader.GetRangesAndIds();

        var sumPart1 = ids
            .Where(id => ranges.Contains(id))
            .Count();

        var sumPart2 = ranges
            .Select(r => r.End - r.Start + 1)
            .Sum();

        
        Common.Result("Number of fresh IDs", sumPart1, 862);
        Common.Result("Total number fresh IDs", sumPart2, 357907198933892UL);
    }

    private static (IEnumerable<Range> Ranges, IEnumerable<ulong> Ids) GetRangesAndIds(this TextReader reader)
    {
        var lines = reader.ReadLines().ToList();

        var ranges = lines
            .Where(s => s.Contains('-'))
            .Select(ParseRanges);


        var ids = lines
            .Skip(ranges.Count() + 1) // skip ranges and blank space
            .Select(s => ulong.Parse(s));

        return (ranges.MergeRanges(), ids);
    }

    private static Range ParseRanges(string input)
    {
        var splitted = input.Split('-');
        var start = ulong.Parse(splitted[0]);
        var end = ulong.Parse(splitted[1]);
        return new Range(start, end);
    }

    private static List<Range> MergeRanges(this IEnumerable<Range> ranges)
    {
        var list = ranges.OrderBy(r => r.Start).ToList();

        var merged = new List<Range>();

        foreach (var r in list)
        {
            if (merged.Count == 0)
            {
                merged.Add(r);
            }
            else
            {
                var last = merged[^1];
                if (r.Start <= last.End + 1)
                {
                    // Overlapping or adjacent -> merge
                    merged[^1] = new(last.Start, Math.Max(last.End, r.End));
                }
                else
                {
                    merged.Add(r);
                }
            }
        }

        return merged;
    }

    private static bool Contains(this IEnumerable<Range> ranges, ulong id) => ranges.Any(r => r.Contains(id));

    private static bool Contains(this Range range, ulong id) => range.Start <= id && id <= range.End;
}
