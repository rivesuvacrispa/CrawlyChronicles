using System.Collections;
using System.Text;
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


        private float duration;
        private float speed;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            duration = LerpLevel(durationLvl1, durationLvl10, lvl);
            speed = LerpLevel(speedLvl1, speedLvl10, lvl);
        }

        public override void Activate()
        {
            StopAllCoroutines();
            StartCoroutine(AbilityRoutine());
        }

        private IEnumerator AbilityRoutine()
        {
            trailRenderer.emitting = true;
            Player.Movement.MoveSpeedAmplifier = speed;
            yield return new WaitForSeconds(duration);
            Player.Movement.MoveSpeedAmplifier = 1;
            trailRenderer.emitting = false;
        }
        
        public override string GetLevelDescription(int lvl)
        {
            StringBuilder sb = new StringBuilder();

            float prevCd = 0;
            float dur = LerpLevel(durationLvl1, durationLvl10, lvl);
            float prevDur = 0;
            float spd = LerpLevel(speedLvl1, speedLvl10, lvl);
            float prevSpd = 0;

            if (lvl > 0)
            {
                int prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevDur = LerpLevel(durationLvl1, durationLvl10, prevLvl);
                prevSpd = LerpLevel(speedLvl1, speedLvl10, prevLvl);
            }

            sb.AddAbilityLine("Cooldown", Scriptable.GetCooldown(lvl), prevCd, false);
            sb.AddAbilityLine("Duration", dur, prevDur);
            sb.AddAbilityLine("Speed multiplier", spd, prevSpd);
            
            return sb.ToString();
        }
    }
}