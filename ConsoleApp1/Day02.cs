using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode;

internal static class Day02
{
    record Range(ulong Start, ulong End);
    record InvalidId(ulong Value, int Repeats);

    public static void Run(TextReader reader)
    {
        // parse and merge ranges
        var input = reader.ReadLine() ?? "";
        var rawRanges = ParseRanges(input);
        var mergedRanges = MergeRanges(rawRanges);

        // global min/max for the domain
        var globalMin = mergedRanges.Min(r => r.Start);
        var globalMax = mergedRanges.Max(r => r.End);

        // generation all invalid ids in [globalMin, ..., globalMax] with their repitition count
        var allInvalid = GenerateAllInvalidIds(globalMin, globalMax);

        // get invalidIDs for tasks
        var part1Values = allInvalid
            .Where(x => x.Repeats == 2)
            .Select(x => x.Value)
            .Distinct();

        var part2Values = allInvalid
            .Select(x => x.Value)
            .Distinct();

        // sum values
        var sumPart1 = part1Values
            .Where(v => mergedRanges.Any(r => r.Start <= v && v <= r.End))
            .Sum();

        var sumPart2 = part2Values
            .Where(v => mergedRanges.Any(r => r.Start <= v && v <= r.End))
            .Sum();

        Console.WriteLine($"Sum of invalid IDs : {sumPart1} --> Is correct ? {sumPart1 == 28844599675}");
        Console.WriteLine($"Sum of invalid IDs : {sumPart2} --> Is correct ? {sumPart2 == 48778605167}");
    }

    private static IEnumerable<Range> ParseRanges(string input) => input
            .Split(',') // split ranges
            .Select(strRange => strRange.Split('-')) // split range pairs
            .Select(strs =>
            {
                var start = ulong.Parse(strs[0]);
                var end = ulong.Parse(strs[1]);
                return new Range(start, end);
            });

    private static int CountDigit(this ulong num)
    {
        if (num == 0) return 1;
        int d = 0;
        while (num > 0)
        {
            d++;
            num /= 10;
        }
        return d;
    }

    private static ulong ComputeRepeatedFactor(int blockLen, int repetitions, ulong[] pow10)
    {
        ulong factor = 0;
        ulong powBlockLen = pow10[blockLen]; // 10^blockLen
        for (int i = 0; i < repetitions; i++)
        {
            factor = factor * powBlockLen + 1;
        }
        return factor;
    }

    private static ulong[] BuildPowersOf10(int maxDigit)
    {
        ulong[] pow10 = new ulong[maxDigit + 1];
        pow10[0] = 1;
        for (int i = 1; i <= maxDigit; i++)
            pow10[i] = pow10[i - 1] * 10;
        return pow10;
    }

    private static List<Range> MergeRanges(IEnumerable<Range> ranges)
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

    private static List<InvalidId> GenerateAllInvalidIds(ulong globalMin, ulong globalMax)
    {
        var maxDigits = globalMax.CountDigit();
        var result = new List<InvalidId>();
        var pow10 = BuildPowersOf10(maxDigits);

        for (int blockLength = 1; blockLength <= maxDigits; blockLength++)
        {
            GenerateForBlockLength(blockLength, globalMin, globalMax, maxDigits, pow10, result);
        }

        return result;
    }

    private static void GenerateForBlockLength(int L, ulong globalMin, ulong globalMax, int maxDigits, ulong[] pow10, List<InvalidId> output)
    {
        var powLMinus1 = pow10[L - 1]; // smallest L-digit number = 10^(L-1)
        var powL = pow10[L]; // 10^L

        for (int m = 2; ; m++)
        {
            int totalDigits = L * m;
            if (totalDigits > maxDigits)
                break;

            var factor = ComputeRepeatedFactor(L, m, pow10);

            // B is an L-digit number: [10^(L-1), 10^L - 1]
            var blockMin = powLMinus1;
            var blockMax = powL - 1;

            // Constrain B so that B * factor lies within [globalMin, globalMax]
            var lowerFromGlobal = (globalMin + factor - 1) / factor; // ceil(globalMin / factor)
            var upperFromGlobal = globalMax / factor; // floor(globalMax / factor)

            if (lowerFromGlobal > blockMin)
                blockMin = lowerFromGlobal;
            if (upperFromGlobal < blockMax)
                blockMax = upperFromGlobal;

            if (blockMin > blockMax)
                continue;

            for (var B = blockMin; B <= blockMax; B++)
            {
                var val = B * factor;
                if (val < globalMin || val > globalMax)
                    continue;

                output.Add(new InvalidId(val, m));
            }
        }
    }
}
