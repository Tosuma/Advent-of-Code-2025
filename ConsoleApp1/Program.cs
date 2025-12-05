using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode;

public class Program
{
    public static void Main()
    {
        Action<TextReader>[] problemSolutions =
        [
            Day01.Run, Day02.Run, Day03.Run
        ];

        foreach ((int fromIndex, int toIndex) in ProblemIndices(problemSolutions.Length))
        {
            if (fromIndex == toIndex)
            {
                problemSolutions[fromIndex](LocateInput(fromIndex));
                continue;
            }

            for (int i = fromIndex; i <= toIndex; i++)
            {
                Console.WriteLine($"Day {i + 1}:");
                Console.WriteLine();

                problemSolutions[i](LocateInput(i));

                if (i < toIndex) Console.WriteLine(new string('-', 80));
            }
        }
    }

    private static TextReader LocateInput(int problemIndex)
    {
        return Directory.GetFiles(Directory.GetCurrentDirectory(), $"Day{problemIndex + 1:D2}.txt", SearchOption.AllDirectories) switch
        {
            [var file, ..] => new StreamReader(file),
            _ => Console.In
        };
    }

    private static IEnumerable<(int from, int to)> ProblemIndices(int length)
    {
        string prompt = $"{Environment.NewLine}Enter the day number [1-{length}] (A = all, ENTER = quit): ";
        Console.Write(prompt);
        while (true)
        {
            string input = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(input)) yield break;
            else if (input == "A" || input == "a") yield return (0, length - 1);
            else if (int.TryParse(input, out int number) && number >= 1 && number <= length) yield return (number - 1, number - 1);

            Console.Write(prompt);
        }
    }
}