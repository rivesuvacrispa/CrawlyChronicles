using System.Collections.Generic;
using System.Linq;
using Gameplay.Genes;
using Gameplay.Player;
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

        public TrioGene CountTakenSlots()
        {
            var keys = mutations.Keys.ToList();
            return new TrioGene(
                keys.Count(m => m.TakesSlot && m.GeneType == GeneType.Aggressive),
                keys.Count(m => m.TakesSlot && m.GeneType == GeneType.Defensive),
                keys.Count(m => m.TakesSlot && m.GeneType == GeneType.Neutral)
            );
        }

        public Dictionary<BasicMutation, int> GetAll() => mutations;
        public MutationData Copy() => new(mutations.ToDictionary(pair => pair.Key, pair => pair.Value));

        public bool CanFitMutation(BasicMutation mutation)
        {
            var keys = mutations.Keys;
            bool alreadyExists = keys.Contains(mutation);
            return alreadyExists ||
                   !mutation.TakesSlot ||
                   keys.Count(m => m.TakesSlot && m.GeneType == mutation.GeneType) <
                   CharacterManager.CurrentCharacter.MutationSlots.GetGene(mutation.GeneType);
        }
    }
}