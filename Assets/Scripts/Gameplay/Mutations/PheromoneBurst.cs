using System.Collections;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class PheromoneBurst : Ability
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
    }
}