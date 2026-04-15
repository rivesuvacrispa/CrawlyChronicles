using System.Collections.Generic;
using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Mutations.Stats;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Random = UnityEngine.Random;

namespace Gameplay.Mutations.Passive
{
    public class GeneAffinity : StatsAbility
    {
        [SerializeField] private GeneType geneType;
        [SerializeField, MinMaxRange(0.01f, 1f)] private LevelFloat bonusChance = new LevelFloat(0.1f, 0.5f);
        
        private float currentBonusChance;


        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            List<ILevelField> fields = new List<ILevelField>()
            {
                bonusChance.UseKey(LevelFieldKeys.BONUS_GENES).UseFormatter(StatFormatter.PERCENT)
            };
            fields.AddRange(base.CreateLevelFields(lvl));
            return fields.ToArray();
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentBonusChance = bonusChance.AtLvl(lvl);
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
            int bonus = 0;
            for (int i = 0; i < amount; i++)
                if (Random.value <= currentBonusChance)
                    bonus++;
            
            if (bonus > 0)
                BreedingManager.Instance.AddGeneDirectly(geneType, bonus);
        }
    }
}