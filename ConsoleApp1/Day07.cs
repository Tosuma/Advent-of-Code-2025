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

    /// <summary>
    /// Simulates the propagation of beams through the specified manifold and calculates the total number of splits and
    /// beams encountered during the simulation.
    /// </summary>
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

    /// <summary>
    /// Calculates the total count by summing the Count property of each Beam in the collection.
    /// </summary>
    private static ulong SumBeams(this IEnumerable<Beam> beams) =>
        beams.Aggregate(0UL, (acc, beam) => acc + beam.Count);

    /// <summary>
    /// Counts the number of beams that, after being moved, end at positions containing splitters within the specified
    /// manifold.
    /// </summary>
    private static int CountSplits(this Manifold manifold, IEnumerable<Beam> beams) =>
        beams
        .Select(Move)
        .Count(beam => manifold.Splitters.Contains(beam.Position));

    /// <summary>
    /// Splits the specified beam into multiple beams based on its position sequence.
    /// </summary>
    private static IEnumerable<Beam> Split(this Beam beam) =>
        beam.Position
        .Split()
        .Select(pos => beam with { Position = pos });

    /// <summary>
    /// Creates two new positions by shifting the specified position one column to the left and one column to the right.
    /// </summary>
    private static IEnumerable<Position> Split(this Position beam) =>
        [beam with { Column = beam.Column - 1 }, beam with { Column = beam.Column + 1 }];

    /// <summary>
    /// Returns a new Beam instance with its position moved according to the Beam's current position.
    /// </summary>
    private static Beam Move(this Beam beam) =>
        beam with { Position = beam.Position.Move() };

    /// <summary>
    /// Returns a new Position that represents the result of moving the specified beam one row forward.
    /// </summary>
    private static Position Move(this Position beam) =>
        beam with { Row = beam.Row + 1 };

    /// <summary>
    /// Moves each beam within the specified manifold, applying splitters and aggregating beams at their resulting
    /// positions.
    /// </summary>
    private static IEnumerable<Beam> Move(this Manifold manifold, IEnumerable<Beam> beams) =>
        beams
        .Select(Move)
        .Where(beam => beam.Position.Row < manifold.RowsCount)
        .SelectMany(beam => manifold.Splitters.Contains(beam.Position) ? beam.Split() : [beam]) // if the beam's location is a splitter split the beam else use same beam
        .GroupBy(beam => beam.Position, (pos, beams) => new Beam(pos, beams.SumBeams()));

    /// <summary>
    /// Parses manifold data from the specified text reader and constructs a corresponding Manifold instance.
    /// </summary>
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

    /// <summary>
    /// Extracts all positions in the specified line where the target character occurs.
    /// </summary>
    private static IEnumerable<Position> Extract(this string line, int row, char target) =>
        line
        .Select((ch, col) => (ch, col))
        .Where(t => t.ch == target)
        .Select(t => new Position(row, t.col));
}
