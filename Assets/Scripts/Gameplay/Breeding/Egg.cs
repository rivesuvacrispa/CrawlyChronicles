using Gameplay.Abilities;
using Genes;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class Egg
    {
        [SerializeField] private TrioGene genes;
        private MutationData mutationData = new();

        public TrioGene Genes => genes;

        public MutationData MutationData => mutationData;

        public Egg(TrioGene genes, MutationData mutationData)
        {
            this.genes = genes;
            this.mutationData = mutationData;
        }

        public Egg()
        {
        }
    }
}