using System.Collections.Generic;
using UnityEngine;

namespace DelaunatorSharp
{
    public struct Edge : IEdge
    {
        public Point P { get; set; }
        public Point Q { get; set; }
        public int Index { get; set; }

        public Edge(int e, Point p, Point q)
        {
            Index = e;
            P = p;
            Q = q;
        }
    }

    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2 AsV2() => new Vector2(X, Y);
        public Vector3 AsV3() => new Vector3(X, Y, 0);
        
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{X},{Y}";

        public override int GetHashCode()
        {
            return ((int) (X * 1000)).GetHashCode() + ((int) (Y * 1000)).GetHashCode();
        }
    }

    public struct Triangle : ITriangle
    {
        public int Index { get; set; }

        public IEnumerable<Point> Points { get; set; }

        public Triangle(int t, IEnumerable<Point> points)
        {
            Points = points;
            Index = t;
        }
    }


}