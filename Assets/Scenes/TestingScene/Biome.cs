using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(menuName = "Generation/Biome")]
public class Biome : ScriptableObject
{
    [SerializeField] private List<Biome> connectsTo = new();
    [SerializeField] private CardinalityDependency cardinalityDependency = CardinalityDependency.None;
    [SerializeField] private Biome southVariant;
    [SerializeField] private Biome northVariant;
    [SerializeField] private new string name;
    [SerializeField] private Color color;
    [SerializeField] private SpriteShape spriteShape;
    
    public string Name => name;
    public Color Color => color;
    public SpriteShape SpriteShape => spriteShape;
    

    public bool CanConnectTo(Biome target) => connectsTo.Contains(target);

    public float GetWeightForCardinality(float cardinalPoint, float disadvantage)
    {
        return cardinalityDependency switch
        {
            CardinalityDependency.None => 1f,
            CardinalityDependency.North => 
                cardinalPoint <= 0 ? 1 + Math.Abs(cardinalPoint) : disadvantage,
            CardinalityDependency.South => 
                cardinalPoint >= 0 ? 1 + cardinalPoint : disadvantage,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public Biome GetVariant(System.Random random, float cardinalPoint, float maxProbability)
    {
        if (southVariant is null && northVariant is null) return this;
        
        if (random.NextDouble() >= Math.Abs(cardinalPoint) * maxProbability) return this;

        return cardinalPoint <= 0 ? 
            northVariant is null ? this : northVariant :
            southVariant is null ? this : southVariant;
    }

    public bool GetVariants(out List<Biome> variants)
    {
        variants = new List<Biome>();
        if (southVariant is not null) variants.Add(southVariant);
        if (northVariant is not null) variants.Add(northVariant);
        return variants.Count > 0;
    }
}
