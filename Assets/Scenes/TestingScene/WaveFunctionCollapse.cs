using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using UnityEngine;

public class WaveFunctionCollapse
{
    private readonly List<VoronoiCell> cells;
    private readonly float maxVariantProbability;
    private readonly Biome universalTile;
    private readonly System.Random random;
    private readonly float cardinalityDisadvantage;

    private int iterationsLeft;

    
    
    public WaveFunctionCollapse(
        List<VoronoiCell> cells,
        List<Biome> superpositions, 
        float maxVariantProbability,
        Biome universalTile, 
        int seed,
        float cardinalityDisadvantage,
        int iterationsLeft)
    {
        this.maxVariantProbability = maxVariantProbability;
        this.universalTile = universalTile;
        this.cells = cells;
        this.cardinalityDisadvantage = cardinalityDisadvantage;
        this.iterationsLeft = iterationsLeft;
        
        foreach (VoronoiCell cell in cells)
        {
            cell.Superpositions = superpositions.ToList();
            cell.Collapsed = false;
        }
        
        random = new System.Random(seed);
    }

    public VoronoiCell LatestCell { get; private set; }


    private void AddResult(VoronoiCell cell)
    {
        if(cell.Entropy == 0)
        {
            cell.Collapse(null);
            return;
        }

        Biome tile = cell.GetPossiblePosition(random, cardinalityDisadvantage);
        Propagate(cell, tile);
        cell.Collapse(tile);
    }

    public List<VoronoiCell> Generate()
    {
        VoronoiCell startingCell = cells.OrderBy(_ => random.NextDouble()).First();
        Collapse(startingCell);
        
        ApplyVariants();
        // GeneratedPotential = CountGeneratedTiles();
        // FillHollows();
        
        return cells;
    }


    /*private void FillHollows()
    {
        for (var x = 0; x < size; x++)
        for (var y = 0; y < size; y++)
            Result[x, y] ??= universalTile;
    }*/

    private void ApplyVariants()
    {
        foreach (VoronoiCell cell in cells) 
            cell.ApplyVariant(random, maxVariantProbability);
    }
    
    private void Collapse(VoronoiCell cell)
    {
        if(cell.Collapsed) return;
        if(iterationsLeft-- <= 0)
        {
            LatestCell = cell;
            return;
        }
        
        AddResult(cell);
        
        if(FindNeighbourOfMinEntropy(cell, out var nextCollapse)) 
            Collapse(nextCollapse);
        else if(FindAnyWithMinEntropy(out nextCollapse))
            Collapse(nextCollapse);

        if (LatestCell is null) LatestCell = cell;
    }

    private void Propagate(VoronoiCell cell, Biome tile)
    {
        foreach (VoronoiCell neighbour in cell.Neighbours)
        {
            if (neighbour.Collapsed) continue;
            
            foreach (Biome possibleNeighbour in neighbour.Superpositions.ToArray())
                if (!possibleNeighbour.CanConnectTo(tile))
                    neighbour.RemovePosition(possibleNeighbour);
        }
    }
    
    private bool FindNeighbourOfMinEntropy(VoronoiCell cell, out VoronoiCell n)
    {
        n = null;
        int minEntropy = int.MaxValue;
        
        foreach (VoronoiCell neighbour in cell.Neighbours.OrderBy(_ => random.NextDouble()))
        {
            if (neighbour.Collapsed) continue;
            
            int sideEntropy = neighbour.Entropy;
            if (sideEntropy < minEntropy)
            {
                minEntropy = sideEntropy;
                n = neighbour;
            }
        }

        return n != null;
    }

    private bool FindAnyWithMinEntropy(out VoronoiCell n)
    {
        int minEntropy = int.MaxValue;
        n = null;
        
        foreach (VoronoiCell cell in cells)
        {
            if (cell.Collapsed) continue;
            
            int entropy = cell.Entropy;
            if (entropy < minEntropy)
            {
                minEntropy = entropy;
                n = cell;
            }
        }
            
        return n != null;
    }
}
