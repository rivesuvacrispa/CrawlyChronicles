using System.Collections;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Mutations.Active
{
    public class PheromoneBurst : ActiveAbility
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [Header("Duration")] 
        [SerializeField] private float durationLvl1;
        [SerializeField] private float durationLvl10;
        [Header("Speed amplifier")]
        [SerializeField] private float speedLvl1;
        [SerializeField] private float speedLvl10;
        [Header("Bonus damage")] 
        [SerializeField, Range(0, 1)] private float damageBoostLvl1;
        [SerializeField, Range(0, 1)] private float damageBoostLvl10;
        
        private float duration;
        private float speed;
        private float damageBoost;

        private PlayerStats activeStats = PlayerStats.Zero;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            duration = LerpLevel(durationLvl1, durationLvl10, lvl);
            speed = LerpLevel(speedLvl1, speedLvl10, lvl);
            damageBoost = LerpLevel(damageBoostLvl1, damageBoostLvl10, lvl);
        }

        public override void Activate()
        {
            StopAllCoroutines();
            PlayerManager.Instance.AddStats(activeStats.Negated());
            StartCoroutine(AbilityRoutine());
        }
        
        private IEnumerator AbilityRoutine()
        {
            trailRenderer.emitting = true;
            PlayerMovement.MoveSpeedAmplifier = speed;
            activeStats = new PlayerStats
                (attackDamage: PlayerManager.PlayerStats.AttackDamage * damageBoost,
                abilityDamage: damageBoost);
            PlayerManager.Instance.AddStats(activeStats);
            
            yield return new WaitForSeconds(duration);

            PlayerMovement.MoveSpeedAmplifier = 1;
            trailRenderer.emitting = false;
            PlayerManager.Instance.AddStats(activeStats.Negated());
            activeStats = PlayerStats.Zero;
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
            int spd = (int) (LerpLevel(speedLvl1, speedLvl10, lvl) * 100) - 100;
            int prevSpd = spd;
            int dmg = (int) (LerpLevel(damageBoostLvl1, damageBoostLvl10, lvl) * 100);
            int prevDmg = dmg;

            if (lvl > 0 && withUpgrade)
            {
                int prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevDur = LerpLevel(durationLvl1, durationLvl10, prevLvl);
                prevSpd = (int) (LerpLevel(speedLvl1, speedLvl10, prevLvl) * 100) - 100;
                prevDmg = (int) (LerpLevel(damageBoostLvl1, damageBoostLvl10, prevLvl) * 100);
            }
            return new object[]
            {
                cd,          dur,           spd,           dmg,
                cd - prevCd, dur - prevDur, spd - prevSpd, dmg - prevDmg
            };
        }
    }
}