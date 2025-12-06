using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode;

internal static class Day04
{
    record Position(int Row, int Column);
    record Map(HashSet<Position> Rolls, Dictionary<Position, List<Position>> Neighbors);
    public static void Run(TextReader reader)
    {
        var map = reader.ReadMap();

        var sumPart1 = map.Rolls
            .Select(pos => map.CountSurroundingRolls(pos))
            .Count(count => count < 4);

        var sumPart2 = map.CountTotalRemovableRolls();

        Console.WriteLine($"Rolls of paper accessible : {sumPart1} --> Is correct ? {sumPart1 == 1433}");
        Console.WriteLine($"Rolls of paper removable  : {sumPart2} --> Is correct ? {sumPart2 == 8616}");
    }

    private static Map ReadMap(this TextReader reader)
    {
        var rows = reader.ReadLines().ToArray();

        var rolls = new HashSet<Position>();
        var neighbors = new Dictionary<Position, List<Position>>();

        for (int row = 0; row < rows.Length; row++)
        {
            for (int column = 0; column < rows[row].Length; column++)
            {
                var position = new Position(row, column);
                var line = rows[row];

                if (rows[row][column] == '@')
                    rolls.Add(position);

                var validNeighbors = new List<Position>();

                // finding neighbors
                if (position.Row > 0 && position.Column > 0)
                    validNeighbors.Add(new Position(position.Row - 1, position.Column - 1));

                if (position.Row > 0)
                    validNeighbors.Add(new Position(position.Row - 1, position.Column));

                if (position.Row > 0 && position.Column < line.Length - 1)
                    validNeighbors.Add(new Position(position.Row - 1, position.Column + 1));

                if (position.Column > 0)
                    validNeighbors.Add(new Position(position.Row, position.Column - 1));

                if (position.Column < line.Length - 1)
                    validNeighbors.Add(new Position(position.Row, position.Column + 1));

                if (position.Row < rows.Length - 1 && position.Column > 0)
                    validNeighbors.Add(new Position(position.Row + 1, position.Column - 1));

                if (position.Row < rows.Length - 1)
                    validNeighbors.Add(new Position(position.Row + 1, position.Column));

                if (position.Row < rows.Length - 1 && position.Column < line.Length - 1)
                    validNeighbors.Add(new Position(position.Row + 1, position.Column + 1));

                neighbors[position] = validNeighbors;
            }
        }

        return new(rolls, neighbors);
    }

    private static int CountSurroundingRolls(this Map map, Position position)
        => map.Neighbors[position].Count(map.Rolls.Contains);

    private static int CountTotalRemovableRolls(this Map map)
    {
        var rolls = map.Rolls.ToHashSet(); // make a copy
        int removedRolls = 0;

        while (true)
        {
            var removable = rolls
                .Where(pos => map.Neighbors[pos].Count(rolls.Contains) < 4);

            if (!removable.Any())
                break;

            foreach (var pos in removable)
            {
                rolls.Remove(pos);
                removedRolls++;
            }
        }

        return removedRolls;
    }
}
