using System.Collections;
using Gameplay.Mutations.Stats;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Mutations.Passive
{
    public class PredatorsHeart : StatsAbility
    {
        [SerializeField] private PlayerManager playerManager;

        [Header("Regen")]
        [SerializeField] private float regenDelay;
        [SerializeField] private float regenLvl1;
        [SerializeField] private float regenLvl10;

        private PlayerStats currentStats = PlayerStats.Zero;
        private float currentRegen;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentRegen = LerpLevel(regenLvl1, regenLvl10, lvl);
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
                yield return new WaitForSeconds(regenDelay);
                PlayerManager.Instance.AddHealthPercent(currentRegen);
            }
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            int regen = (int) (LerpLevel(regenLvl1, regenLvl10, lvl) * 100);
            int prevRegen = regen;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevRegen = (int) (LerpLevel(regenLvl1, regenLvl10, prevLvl) * 100);
            }
            
            var args = new object[]
            {
                regen,
                regen - prevRegen,
                regenDelay
            };
            
            return  scriptable.GetStatDescription(args) + "\n" + base.GetLevelDescription(lvl, withUpgrade);
        }
    }
}