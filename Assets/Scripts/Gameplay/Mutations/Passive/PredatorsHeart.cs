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
            float hp = LerpLevel(healthLvl1, healthLvl10, lvl);
            float prevhp = hp;
            int regen = (int) (LerpLevel(regenLvl1, regenLvl10, lvl) * 100);
            int prevRegen = regen;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevhp = LerpLevel(healthLvl1, healthLvl10, prevLvl);
                prevRegen = (int) (LerpLevel(regenLvl1, regenLvl10, prevLvl) * 100);
            }
            
            var args = new object[]
            {
                hp,          regen,
                hp - prevhp, regen - prevRegen,
                regenDelay
            };
            
            return scriptable.GetStatDescription(args);
        }
    }
}