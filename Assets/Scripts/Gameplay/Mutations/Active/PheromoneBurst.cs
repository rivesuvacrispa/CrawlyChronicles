using System.Collections;
using System.Text;
using Player;
using UnityEngine;
using Util;

namespace Gameplay.Abilities.Active
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

        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            StringBuilder sb = new StringBuilder();

            float prevCd = 0;
            float dur = LerpLevel(durationLvl1, durationLvl10, lvl);
            float prevDur = 0;
            float spd = LerpLevel(speedLvl1, speedLvl10, lvl);
            float prevSpd = 0;
            float dmg = LerpLevel(damageBoostLvl1, damageBoostLvl10, lvl);
            float prevDmg = 0;

            if (lvl > 0 && withUpgrade)
            {
                int prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevDur = LerpLevel(durationLvl1, durationLvl10, prevLvl);
                prevSpd = LerpLevel(speedLvl1, speedLvl10, prevLvl);
                prevDmg = LerpLevel(damageBoostLvl1, damageBoostLvl10, prevLvl);
            }

            sb.AddAbilityLine("Cooldown", Scriptable.GetCooldown(lvl), prevCd, false, suffix: "s");
            sb.AddAbilityLine("Duration", dur, prevDur, suffix: "s");
            sb.AddAbilityLine("Speed boost", spd, prevSpd, percent: true);
            sb.AddAbilityLine("Damage boost", dmg, prevDmg, percent: true);
            
            return sb.ToString();
        }
    }
}