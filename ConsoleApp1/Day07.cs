using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode;

internal static class Day07
{
    record struct Beam(Position Position, ulong Count);
    record struct Position(int Row, int Column);
    record struct Manifold(int RowsCount, Position Start, HashSet<Position> Splitters);
    public static void Run(TextReader reader)
    {
        var manifold = reader.ReadManifold();

        var (totalSplits, totalBeams) = manifold.Simulate();

        Common.Result("Total splits", totalSplits, 1543);
        Common.Result("Total beams", totalBeams, 3223365367809UL);
    }

    private static (int totalSplits, ulong totalBeams) Simulate(this Manifold manifold)
    {

        List<Beam> beams = [new Beam(manifold.Start, 1)];
        var totalSplits = 0;
        var totalBeams = beams.SumBeams();

        while (true)
        {
            totalSplits += manifold.CountSplits(beams);
            var nextBeams = manifold.Move(beams).ToList();
            if (nextBeams.Count == 0)
                break;

            beams = nextBeams;
            totalBeams = beams.SumBeams();
        }

        return (totalSplits, totalBeams);
    }

    private static ulong SumBeams(this IEnumerable<Beam> beams) =>
        beams.Aggregate(0UL, (acc, beam) => acc + beam.Count);

    private static int CountSplits(this Manifold manifold, IEnumerable<Beam> beams) =>
        beams
        .Select(Move)
        .Count(beam => manifold.Splitters.Contains(beam.Position));

    private static IEnumerable<Beam> Split(this Beam beam) =>
        beam.Position
        .Split()
        .Select(pos => beam with { Position = pos });

    private static IEnumerable<Position> Split(this Position beam) =>
        [beam with { Column = beam.Column - 1 }, beam with { Column = beam.Column + 1 }];

    private static Beam Move(this Beam beam) =>
        beam with { Position = beam.Position.Move() };

    private static Position Move(this Position beam) =>
        beam with { Row = beam.Row + 1 };

    private static IEnumerable<Beam> Move(this Manifold manifold, IEnumerable<Beam> beams) =>
        beams
        .Select(Move)
        .Where(beam => beam.Position.Row < manifold.RowsCount)
        .SelectMany(beam => manifold.Splitters.Contains(beam.Position) ? beam.Split() : [beam])
        .GroupBy(beam => beam.Position, (pos, beams) => new Beam(pos, beams.SumBeams()));

    private static Manifold ReadManifold(this TextReader reader)
    {
        int rowsCount = 0;
        Position? start = null;
        HashSet<Position> splitters = [];

        foreach (var line in reader.ReadLines())
        {
            // find the start point
            foreach (var pos in line.Extract(rowsCount, 'S'))
                start = pos;

            // find the splitters
            foreach (var pos in line.Extract(rowsCount, '^'))
                splitters.Add(pos);

            rowsCount++;
        }

        if (!start.HasValue)
            throw new InvalidDataException("Start position not found.");

        return new Manifold(rowsCount, start.Value, splitters);
    }

    private static IEnumerable<Position> Extract(this string line, int row, char target) =>
        line
        .Select((ch, col) => (ch, col))
        .Where(t => t.ch == target)
        .Select(t => new Position(row, t.col));
}
