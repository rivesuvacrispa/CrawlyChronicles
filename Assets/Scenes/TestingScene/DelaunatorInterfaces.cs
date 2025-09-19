using System.Collections.Generic;
using UnityEngine;

namespace DelaunatorSharp
{
    public interface IEdge
    {
        Point P { get; }
        Point Q { get; }
        int Index { get; }
    }
    
    public interface ITriangle
    {
        IEnumerable<Point> Points { get; }
        int Index { get; }
    }
}