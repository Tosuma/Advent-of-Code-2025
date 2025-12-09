using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode;

internal static class Day08
{
    record Vector(int X, int Y, int Z);
    record Ray(Vector From, Vector To);
    public static void Run(TextReader reader)
    {
        var positions = reader.ReadVectors().ToList();


        Common.Result("Largest circuits", 123, 123);
    }



    private static IEnumerable<Vector> ReadVectors(this TextReader reader) =>
        reader
            .ReadLines()
            .Select(line => line.Split(',').Select(int.Parse).ToArray())
            .Select(coords => new Vector(coords[0], coords[1], coords[2]));

    private static long Distance(this Vector a, Vector b) =>
        (long)(a.X - b.X) * (a.X - b.X) +
        (long)(a.Y - b.Y) * (a.Y - b.Y) +
        (long)(a.Z - b.Z) * (a.Z - b.Z);
}
