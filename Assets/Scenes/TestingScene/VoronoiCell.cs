using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using Unity.VisualScripting;
using UnityEngine;

public class VoronoiCell
{
    public List<Biome> Superpositions { get; set; }
    public bool Collapsed { get; set; }
    public Point[] Points { get; private set; }
    public int Index { get; }
    public HashSet<VoronoiCell> Neighbours { get; set; }
    public bool MarkedForMerge { private set; get; }
    public int Entropy => Superpositions.Count;
    public Biome Biome => Collapsed ? Superpositions[0] : null;
    public float Cardinality => Mathf.Clamp01(Points.Sum(p => p.Y) / Points.Length) - 0.5f;


    public VoronoiCell(int triangleIndex, Point[] points)
    {
        Points = points;
        Index = triangleIndex;
        Neighbours = new HashSet<VoronoiCell>();
        Superpositions = new List<Biome>();
    }
    
    public Biome GetPossiblePosition(System.Random random, float cardinalityDisadvantage)
    {
        int entropy = Entropy;
        float[] weights = new float[entropy];

        float sum = 0;

        for (var i = 0; i < entropy; i++)
        {
            sum += Superpositions[i].GetWeightForCardinality(Cardinality, cardinalityDisadvantage);
            weights[i] = sum;
        }

        float rnd = (float) random.NextDouble() * sum;
        int index = 0;
        for (var i = 0; i < entropy; i++)
        {
            index = i;
            if (weights[i] > rnd) break;
        }
            
            
        return Superpositions[index];
    }

    public void RemovePosition(Biome tile) => Superpositions.Remove(tile);

    public void Collapse(Biome tile)
    {
        Superpositions = new List<Biome> {tile};
        Collapsed = true;
    }

    public void ApplyVariant(System.Random random, float probability)
    {
        if (!Collapsed) return;
        
        Superpositions[0] = Biome.GetVariant(random, Cardinality, probability);
    }

    public void TryMergeWithNeighbours(in HashSet<VoronoiCell> winners, VoronoiCell previous = null)
    {
        if (MarkedForMerge) return;
        MarkedForMerge = true;
        
        foreach (VoronoiCell n in Neighbours.ToArray().Where(n 
                     => !n.MarkedForMerge && Biome.Equals(n.Biome)))
        {
            n.TryMergeWithNeighbours(winners, this);
        }
            
        
        if (previous is not null)
        {
            previous.MergeWith(this);

            Neighbours.Clear();
            Points = null;
            Superpositions = null;
            Collapsed = false;
            
            return;
        }
        
        winners.Add(this);
    }

    private void MergeWith(VoronoiCell target)
    {
        var connectedPoints = new HashSet<Point>(Points);
        connectedPoints.IntersectWith(target.Points);

        int current = 0;
        foreach (var p in Points)
        {
            // bruh bruh
        }
        
        var combinedPoints = new HashSet<Point>(Points);
        foreach (Point newPoint in target.Points) 
            combinedPoints.Add(newPoint);
        Points = combinedPoints.ToArray();
        
        foreach (VoronoiCell neighbour in target.Neighbours) 
            Neighbours.Add(neighbour);
    }
}