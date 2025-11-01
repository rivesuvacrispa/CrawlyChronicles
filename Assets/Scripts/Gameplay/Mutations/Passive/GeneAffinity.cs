using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Mutations.Stats;
using UnityEngine;

namespace Gameplay.Mutations.Passive
{
    public class GeneAffinity : StatsAbility
    {
        [SerializeField] private GeneType geneType;
        [Header("Bonus Chance")]
        [SerializeField, Range(0.01f, 1f)] private float bonusChanceLvl1;
        [SerializeField, Range(0.01f, 1f)] private float bonusChanceLvl10;
        
        private float bonusChance;


        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            bonusChance = LerpLevel(bonusChanceLvl1, bonusChanceLvl10, lvl);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BreedingManager.OnBeforeGenePickup += OnBeforeGenePickup;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BreedingManager.OnBeforeGenePickup -= OnBeforeGenePickup;
        }

        private void OnBeforeGenePickup(GeneType gType, int amount)
        {
            if (amount == 0) return;
            
            int bonus = 0;
            for (int i = 0; i < amount; i++)
                if (Random.value <= bonusChance)
                    bonus++;
            
            if (bonus > 0)
                BreedingManager.Instance.AddGeneDirectly(geneType, bonus);
        }
    }
}