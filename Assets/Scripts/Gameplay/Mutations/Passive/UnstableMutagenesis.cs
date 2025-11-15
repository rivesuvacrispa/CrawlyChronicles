using Gameplay.Mutations.Stats;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Mutations.Passive
{
    public class UnstableMutagenesis : StatsAbility
    {
        [SerializeField, Range(0, 1f)] private float downgradeChance;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            MutationMenu.OnMutationClick += OnMutationClick;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MutationMenu.OnMutationClick -= OnMutationClick;
        }

        private void OnMutationClick()
        {
            if (Random.value <= downgradeChance)
            {
                MutationMenu.BreakRandomMutation();
            }
        }
    }
}