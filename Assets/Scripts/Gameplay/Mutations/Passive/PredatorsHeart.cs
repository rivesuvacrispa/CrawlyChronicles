using System.Collections;
using System.Text;
using Player;
using UnityEngine;
using Util;

namespace Gameplay.Abilities.Passive
{
    public class PredatorsHeart : BasicAbility
    {
        [SerializeField] private PlayerManager playerManager;
        [Header("Max health")]
        [SerializeField] private float healthLvl1;
        [SerializeField] private float healthLvl10;

        [Header("Regen")]
        [SerializeField] private float regenDelay;
        [SerializeField] private float regenLvl1;
        [SerializeField] private float regenLvl10;

        private PlayerStats currentStats = PlayerStats.Zero;
        private float currentRegen;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(!currentStats.Equals(PlayerStats.Zero) && Application.isPlaying) playerManager.AddStats(currentStats.Negated());
            currentStats = new PlayerStats(maxHealth: LerpLevel(healthLvl1, healthLvl10, lvl));
            if(Application.isPlaying) playerManager.AddStats(currentStats);
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
            while (gameObject.activeInHierarchy && enabled)
            {
                yield return new WaitForSeconds(regenDelay);
                PlayerManager.Instance.AddHealthPercent(currentRegen);
            }
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            StringBuilder sb = new StringBuilder();

            float hp = LerpLevel(healthLvl1, healthLvl10, lvl);
            float prevhp = 0;
            float regen = LerpLevel(regenLvl1, regenLvl10, lvl);
            float prevRegen = 0;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevhp = LerpLevel(healthLvl1, healthLvl10, prevLvl);
                prevRegen = LerpLevel(regenLvl1, regenLvl10, prevLvl);
            }
            
            sb.AddAbilityLine("Max health", hp, prevhp);
            sb.AddAbilityLine("Healing", regen, prevRegen, percent: true);
            sb.AddAbilityLine("Healing tickrate", regenDelay, 0, suffix: "s");
            
            return sb.ToString();
        }
    }
}