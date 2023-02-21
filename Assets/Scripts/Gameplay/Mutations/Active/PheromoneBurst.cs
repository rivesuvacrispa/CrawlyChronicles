using System.Collections;
using System.Text;
using UnityEngine;

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
            float dur = LerpLevel(durationLvl1, durationLvl10, lvl);
            float spd = LerpLevel(speedLvl1, speedLvl10, lvl);
            StringBuilder sb = new StringBuilder();
            sb.Append("<color=orange>").Append("Cooldown").Append(": ").Append("</color>").Append(Scriptable.GetCooldown(lvl).ToString("n2")).Append("\n");
            sb.Append("<color=orange>").Append("Duration").Append(": ").Append("</color>").Append(dur.ToString("n2")).Append("\n");
            sb.Append("<color=orange>").Append("Speed multiplier").Append(": ").Append("</color>").Append(spd.ToString("n2")).Append("\n");
            return sb.ToString();
        }
    }
}