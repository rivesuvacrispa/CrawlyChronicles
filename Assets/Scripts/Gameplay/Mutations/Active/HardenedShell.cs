using System.Collections;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Mutations.Active
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
            PlayerManager.Instance.AddStats(activeStats.Negated());
            StartCoroutine(AbilityRoutine());
        }

        private IEnumerator AbilityRoutine()
        {
            particleSystem.Play();
            activeStats = new PlayerStats
                (armor: PlayerManager.PlayerStats.Armor * armor,
                passiveProcRate: procChance);
            PlayerManager.Instance.AddStats(activeStats);

            yield return new WaitForSeconds(duration);

            PlayerManager.Instance.AddStats(activeStats.Negated());
            activeStats = PlayerStats.Zero;
            particleSystem.Stop();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAllCoroutines();
            PlayerManager.Instance.AddStats(activeStats.Negated());
        }
        
        public override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            float cd = Scriptable.GetCooldown(lvl);
            float prevCd = cd;
            float dur = LerpLevel(durationLvl1, durationLvl10, lvl);
            float prevDur = dur;
            int arm = (int) (LerpLevel(armorLvl1, armorLvl10, lvl) * 100);
            int prevArm = arm;
            int proc = (int) (LerpLevel(procChanceLvl1, procChanceLvl10, lvl) * 100);
            int prevProc = proc;

            if (lvl > 0 && withUpgrade)
            {
                int prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevDur = LerpLevel(durationLvl1, durationLvl10, prevLvl);
                prevArm = (int) (LerpLevel(armorLvl1, armorLvl10, prevLvl) * 100);
                prevProc = (int) (LerpLevel(procChanceLvl1, procChanceLvl10, prevLvl) * 100);
            }
            
            return new object[]
            {
                cd,          dur,           arm,           proc,
                cd - prevCd, dur - prevDur, arm - prevArm, proc - prevProc
            };
        }
    }
}