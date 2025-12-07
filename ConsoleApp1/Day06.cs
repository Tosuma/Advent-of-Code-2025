using System.Text;

namespace AdventOfCode;

internal static class Day06
{
    record OperationParing(string Operation, IEnumerable<int> Numbers);
    public static void Run(TextReader reader)
    {
        var input = reader.ReadLines().ToArray();
        var pairs = GetOperationParing(input);
        var sumPart1 = pairs
            .Select(ComputePair)
            .Sum();

        var sumPart2 = input.ParseVertically().ComputeHorizontally();

        Common.Result("Grand total by horizontally", sumPart1, 3785892992137UL);
        Common.Result("Grand total by vertically", sumPart2, 7669802156452UL);
    }

    private static IEnumerable<OperationParing> GetOperationParing(IEnumerable<string> input)
    {
        var numbers = input
            .Take(4)
            .Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(line => line.Select(s => int.Parse(s)))
            .Transpose()
            .ToArray();

        var operations = input
            .Skip(4)
            .First()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        for (int i = 0; i < operations.Length; i++)
        {
            yield return new OperationParing(operations[i], numbers[i]);
        }
    }

    private static IEnumerable<IEnumerable<int>> Transpose(this IEnumerable<IEnumerable<int>> source)
    {
        var matrix = source.Select(row => row.ToArray()).ToArray();

        int rowCount = matrix.Length;
        int colCount = matrix[0].Length;

        return Enumerable.Range(0, colCount)
            .Select(col => Enumerable.Range(0, rowCount)
                .Select(row => matrix[row][col])
            );
    }

    private static ulong ComputePair(this OperationParing pair)
    {
        ulong result = 1;
        if (pair.Operation is "*")
            foreach (ulong num in pair.Numbers)
                result *= num;
        else
        {
            result = 0;
            foreach (ulong num in pair.Numbers)
                result += num;
        }

        return result;
    }

    private static IEnumerable<string> ParseVertically(this string[] rawInput)
    {
        for (int col = rawInput.Max(line => line.Length) - 1; col >= 0; col--)
        {
            int row = 0;
            while (row < rawInput.Length)
            {
                while (row < rawInput.Length && (col >= rawInput[row].Length || rawInput[row][col] == ' ')) row++;

                var segment = new StringBuilder();
                while (row < rawInput.Length && col < rawInput[row].Length && rawInput[row][col] != ' ')
                {
                    if ("*+".Contains(rawInput[row][col]) && segment.Length > 0)
                    {
                        yield return segment.ToString();
                        segment.Clear();
                    }
                    segment.Append(rawInput[row++][col]);
                }

                if (segment.Length > 0) yield return segment.ToString();
            }
        }

    }

    private static ulong ComputeHorizontally(this IEnumerable<string> fields)
    {
        ulong sum = 0;
        List<ulong> pending = [];

        foreach (var field in fields)
        {
            if (field is "*")
            {
                sum += pending.Aggregate(1UL, (acc, val) => acc * val);
                pending.Clear();
            }
            else if (field is "+")
            {
                sum += pending.Aggregate(0UL, (acc, val) => acc + val);
                pending.Clear();
            }
            else if (ulong.TryParse(field, out var number))
            {
                pending.Add(number);
            }
            else
            {
                throw new InvalidDataException($"Invalid field: {field}");
            }
        }

        return sum;
    }
}
