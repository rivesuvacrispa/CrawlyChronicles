using System.Collections;
using System.Text;
using Player;
using UnityEngine;
using Util;

namespace Gameplay.Abilities.Active
{
    public class HardenedShell : ActiveAbility
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [Header("Duration")] 
        [SerializeField] private float durationLvl1;
        [SerializeField] private float durationLvl10;
        [Header("Bonus armor")] 
        [SerializeField, Range(0, 1)] private float armorLvl1;
        [SerializeField, Range(0, 1)] private float armorLvl10;
        [Header("Bonus proc chance")] 
        [SerializeField, Range(0, 1)] private float procChanceLvl1;
        [SerializeField, Range(0, 1)] private float procChanceLvl10;
        
        private float duration;
        private float armor;
        private float procChance;

        private PlayerStats activeStats = PlayerStats.Zero;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            duration = LerpLevel(durationLvl1, durationLvl10, lvl);
            armor = LerpLevel(armorLvl1, armorLvl10, lvl);
            procChance = LerpLevel(procChanceLvl1, procChanceLvl10, lvl);
        }

        public override void Activate()
        {
            StopAllCoroutines();
            Manager.Instance.AddStats(activeStats.Negated());
            StartCoroutine(AbilityRoutine());
        }

        private IEnumerator AbilityRoutine()
        {
            particleSystem.Play();
            activeStats = new PlayerStats
                (armor: Manager.PlayerStats.Armor * armor,
                passiveProcRate: procChance);
            Manager.Instance.AddStats(activeStats);

            yield return new WaitForSeconds(duration);

            Manager.Instance.AddStats(activeStats.Negated());
            activeStats = PlayerStats.Zero;
            particleSystem.Stop();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAllCoroutines();
            Manager.Instance.AddStats(activeStats.Negated());
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            StringBuilder sb = new StringBuilder();

            float prevCd = 0;
            float dur = LerpLevel(durationLvl1, durationLvl10, lvl);
            float prevDur = 0;
            float arm = LerpLevel(armorLvl1, armorLvl10, lvl);
            float prevArm = 0;
            float proc = LerpLevel(procChanceLvl1, procChanceLvl10, lvl);
            float prevProc = 0;

            if (lvl > 0 && withUpgrade)
            {
                int prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevDur = LerpLevel(durationLvl1, durationLvl10, prevLvl);
                prevArm = LerpLevel(armorLvl1, armorLvl10, prevLvl);
                prevProc = LerpLevel(procChanceLvl1, procChanceLvl10, lvl);
            }

            sb.AddAbilityLine("Cooldown", Scriptable.GetCooldown(lvl), prevCd, false, suffix: "s");
            sb.AddAbilityLine("Duration", dur, prevDur, suffix: "s");
            sb.AddAbilityLine("Armor boost", arm, prevArm, percent: true);
            sb.AddAbilityLine("Proc chance boost", proc, prevProc, percent: true);
            
            return sb.ToString();
        }
    }
}