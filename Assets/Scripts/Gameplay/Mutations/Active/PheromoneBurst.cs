using System.Collections;
using Gameplay.Player;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class PheromoneBurst : ActiveAbility
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField, MinMaxRange(1f, 10f)] private LevelFloat duration = new LevelFloat(new Vector2(2f, 6f));
        [SerializeField, MinMaxRange(1f, 10f)] private LevelFloat bonusSpeed = new LevelFloat(new Vector2(0.2f, 2f));
        [SerializeField, MinMaxRange(0f, 1f)] private LevelFloat bonusDamage = new LevelFloat(new Vector2(0.15f, 1f));

        
        private float currentDuration;
        private float currentBonusSpeed;
        private float currentBonusDamage;

        private PlayerStats activeStats = PlayerStats.Zero;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentDuration = duration.AtLvl(lvl);
            currentBonusSpeed = bonusSpeed.AtLvl(lvl);
            currentBonusDamage = bonusDamage.AtLvl(lvl);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            StopAllCoroutines();
            PlayerManager.Instance.AddStats(activeStats.Negated());
            StartCoroutine(AbilityRoutine());
        }
        
        // TODO: convert to unitask
        private IEnumerator AbilityRoutine()
        {
            trailRenderer.emitting = true;
            PlayerMovement.MoveSpeedAmplifier = currentBonusSpeed;
            activeStats = new PlayerStats
                (attackDamage: PlayerManager.PlayerStats.AttackDamage * currentBonusDamage,
                abilityDamage: currentBonusDamage);
            PlayerManager.Instance.AddStats(activeStats);
            
            yield return new WaitForSeconds(currentDuration);

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

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                duration.UseKey(LevelFieldKeys.EFFECT_DURATION).UseFormatter(StatFormatter.SECONDS),
                bonusSpeed.UseKey(LevelFieldKeys.BONUS_SPEED),
                bonusDamage.UseKey(LevelFieldKeys.BONUS_DAMAGE).UseFormatter(StatFormatter.PERCENT),
            };
        }
    }
}