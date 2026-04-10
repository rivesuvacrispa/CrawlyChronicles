using Gameplay.Mutations.Stats;
using Gameplay.Player;
using UnityEngine;
using Util;

namespace Gameplay.Mutations.Passive
{
    public class UnstableMutagenesis : StatsAbility
    {
        [SerializeField, Range(0, 1f)] private float downgradeChance;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            MutationManager.OnCollectBreakChance += CollectBreakChance;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MutationManager.OnCollectBreakChance -= CollectBreakChance;
        }

        private void CollectBreakChance(FloatWrapper floatWrapper)
        {
            floatWrapper.Add(downgradeChance);
        }
    }
}