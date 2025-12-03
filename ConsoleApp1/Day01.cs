using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode;

static class Day01
{
    public static void Run(TextReader reader)
    {
        var trace = reader.ReadLines().HandleLine().ToList();

        int zeroEndings = trace.Count(step => step.after == 0);
        int zeroPasses = trace.Sum(step =>
        {
            int leftCrossZero  = step.before > 0 && step.before + (step.move % 100) <= 0 ? 1 : 0;
            int rightCrossZero = step.before > 0 && step.before + (step.move % 100) >= 100 ? 1 : 0;
            int fullCircle     = Math.Abs(step.move) / 100;
            
            return leftCrossZero + rightCrossZero + fullCircle;
        });


        Console.WriteLine($"The number of zero endings : {zeroEndings} --> Is correct ? {zeroEndings == 982}");
        Console.WriteLine($"The number of zero passes  : {zeroPasses} --> Is correct ? {zeroPasses == 6106}");
    }

    private static int ParseMove(string line) => line[0] == 'L' ? -int.Parse(line[1..]) : int.Parse(line[1..]);

    private static IEnumerable<(int before, int move, int after)> HandleLine(this IEnumerable<string> lines)
    {
        int location = 50;
        foreach (var line in lines)
        {
            int before = location;
            int move = ParseMove(line);
            location = ((location + move) % 100 + 100) % 100;

            yield return (before, move, location);
        }
    }
}