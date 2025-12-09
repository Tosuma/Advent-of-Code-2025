using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode;

internal static class Day09
{
    record Point(int Row, int Column);
    record Square
    {
        public Point First { get; }
        public Point Second { get; }
        public long Area { get; }

        public Square(Point p1, Point p2)
        {
            bool p1BeforeP2 =
                p1.Row < p2.Row ||
                (p1.Row == p2.Row && p1.Column <= p2.Column);

            (First, Second) = p1BeforeP2 ? (p1, p2) : (p2, p1);
        }
    }
    public static void Run(TextReader reader)
    {
        var points = reader.ReadPoints().ToList();
        var squares = points.GetSquares();

        var maxSquare = squares
            .Select(GetArea)
            .Max();

        Common.Result("Largest square area (any tiles)", maxSquare, 4745816424);
    }

    private static IEnumerable<Point> ReadPoints(this TextReader reader) =>
        reader.ReadLines()
        .Select(line => line.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
        .Select(nums => new Point(nums[0], nums[1]));

    private static HashSet<Square> GetSquares(this IList<Point> points)
    {
        var squares = new HashSet<Square>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                squares.Add(new Square(points[i], points[j]));
            }
        }

        return squares;
    }

    private static long GetArea(Square square)
    {
        long height = Math.Abs((long)square.First.Row - square.Second.Row) + 1;
        long width = Math.Abs((long)square.First.Column - square.Second.Column) + 1;
        return height * width;
    }
}