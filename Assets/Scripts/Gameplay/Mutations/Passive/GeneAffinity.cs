using Gameplay.Genes;
using UnityEngine;

namespace Gameplay.Mutations.Passive
{
    public class GeneAffinity : BasicAbility
    {
        [SerializeField] private GeneType geneType;
        [SerializeField, Range(0f, 1f)] private float conversionChanceLvl1;
        [SerializeField, Range(0f, 1f)] private float conversionChanceLvl10;
        
        private float conversionChance;


        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            conversionChance = LerpLevel(conversionChanceLvl1, conversionChanceLvl10, lvl);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GeneDrop.OnGeneDropReplaceTypeRequested += OnGeneDropReplaceTypeRequested;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GeneDrop.OnGeneDropReplaceTypeRequested -= OnGeneDropReplaceTypeRequested;
        }

        private GeneType OnGeneDropReplaceTypeRequested()
        {
            return Random.value <= conversionChance ? geneType : default;
        }
    }
}