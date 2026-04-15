using System.Collections;
using System.Collections.Generic;
using Gameplay.Mutations.Stats;
using Gameplay.Player;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Passive
{
    public class PredatorsHeart : StatsAbility
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private LevelConst regenInterval = new LevelConst(10);
        [SerializeField, MinMaxRange(0f, 1f)] private LevelFloat regen = new LevelFloat(0.05f, 0.2f);

        private PlayerStats currentStats = PlayerStats.Zero;
        private float currentRegen;

        

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            List<ILevelField> fields = new List<ILevelField>()
            {
                regenInterval.UseKey(LevelFieldKeys.EFFECT_INTERVAL).UseFormatter(StatFormatter.SECONDS),
                regen.UseKey(LevelFieldKeys.HEALTH_AMOUNT).UseFormatter(StatFormatter.PERCENT)
            };
            fields.AddRange(base.CreateLevelFields(lvl));
            return fields.ToArray();
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentRegen = regen.AtLvl(lvl);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(RegenerationRoutine());
            if(!currentStats.Equals(PlayerStats.Zero)) playerManager.AddStats(currentStats);
        }
        
        protected override void OnDisable()
        {
            playerManager.AddStats(currentStats.Negated());
            base.OnDisable();
        }

        private IEnumerator RegenerationRoutine()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(regenInterval.Value);
                PlayerManager.Instance.AddHealthPercent(currentRegen);
            }
        }
    }
}