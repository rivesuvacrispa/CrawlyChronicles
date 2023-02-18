using System.Collections.Generic;
using Scriptable;

namespace Gameplay.Abilities
{
    public class MutationData
    {
        private readonly Dictionary<BasicMutation, int> mutations = new();

        public void Add(BasicMutation mutation, int lvl) => mutations.Add(mutation, lvl);
        public Dictionary<BasicMutation, int> GetAll() => mutations;

        public MutationData Randomize() => this;

        public MutationData(Dictionary<BasicMutation, int> mutations)
        {
            this.mutations = mutations;
        }

        public MutationData()
        {
        }
    }
}