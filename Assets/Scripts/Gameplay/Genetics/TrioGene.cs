using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Genetics
{
    [Serializable]
    public struct TrioGene
    {
        [SerializeField] private uint[] trio;

        public uint GetGene(GeneType geneType) => trio[(int) geneType];
        public void SetGene(GeneType geneType, uint amount) => trio[(int) geneType] = amount;
        public void AddGene(GeneType geneType) => trio[(int) geneType]++;
        
        public uint Aggressive
        {
            get => trio[0];
            set => trio[0] = value;
        }

        public uint Defensive
        {
            get => trio[1];
            set => trio[1] = value;
        }

        public uint Universal
        {
            get => trio[2];
            set => trio[2] = value;
        }

        public TrioGene(uint aggressive, uint defensive, uint universal)
        {
            trio = new[]
            {
                aggressive,
                defensive,
                universal
            };
        }

        public TrioGene MergeMax(TrioGene other)
        {
            return new TrioGene(
                    Math.Max(Aggressive, other.Aggressive),
                    Math.Max(Defensive, other.Defensive),
                    Math.Max(Universal, other.Universal)
                );
        }

        public static TrioGene Zero => new TrioGene(0, 0, 0);
        public static TrioGene One => new TrioGene(1, 1, 1);
    }
}