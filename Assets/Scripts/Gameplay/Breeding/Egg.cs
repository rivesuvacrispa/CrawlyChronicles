using Gameplay.Genes;
using Gameplay.Mutations;
using UnityEngine;

namespace Gameplay.Breeding
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