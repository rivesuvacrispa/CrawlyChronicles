using System.Collections.Generic;
using System.Linq;
using Gameplay.Genes;
using Scriptable;

namespace Gameplay.Mutations
{
    public class MutationData
    {
        private readonly Dictionary<BasicMutation, int> mutations = new();


        public MutationData()
        {
        }

        private MutationData(Dictionary<BasicMutation, int> mutations)
        {
            this.mutations = mutations;
        }

        public void Add(BasicMutation mutation, int lvl)
        {
            if (mutations.ContainsKey(mutation)) return;
            mutations.Add(mutation, lvl);
        }
        
        public void Set(BasicMutation mutation, int lvl)
        {
            mutations[mutation] = lvl;
        }

        public bool Remove(BasicMutation mutation)
        {
            return mutations.Remove(mutation);
        }

        public bool TryGet(BasicMutation mutation, out int lvl)
        {
            lvl = 0;
            if (!mutations.ContainsKey(mutation)) return false;
            lvl = mutations[mutation];
            return true;
        }

        public TrioGene CountByType()
        {
            var keys = mutations.Keys.ToList();
            return new TrioGene(
                keys.Count(m => m.GeneType == GeneType.Aggressive),
                keys.Count(m => m.GeneType == GeneType.Defensive),
                keys.Count(m => m.GeneType == GeneType.Neutral)
            );
        }
        public Dictionary<BasicMutation, int> GetAll() => mutations;
        public MutationData Copy() => new(mutations.ToDictionary(pair => pair.Key, pair => pair.Value));
    }
}