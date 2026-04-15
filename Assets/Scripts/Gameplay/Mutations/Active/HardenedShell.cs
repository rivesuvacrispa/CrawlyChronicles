using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class HardenedShell : ActiveAbility
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField, MinMaxRange(0, 10)] private LevelFloat duration = new LevelFloat(new Vector2(3f, 5f));
        [SerializeField, MinMaxRange(0f, 1)] private LevelFloat bonusArmor = new LevelFloat(new Vector2(0.1f, 1f));
        [SerializeField, MinMaxRange(0f, 1)] private LevelFloat bonusProcChance = new LevelFloat(new Vector2(0.15f, 1f));

        
        private float currentDuration;
        private float currentBonusArmor;
        private float currentBonusProcChance;

        private PlayerStats activeStats = PlayerStats.Zero;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentDuration = duration.AtLvl(lvl);
            currentBonusArmor = bonusArmor.AtLvl(lvl);
            currentBonusProcChance = bonusProcChance.AtLvl(lvl);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            PlayerManager.Instance.AddStats(activeStats.Negated());
            ActivateTask(CreateCommonCancellationToken()).Forget();
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            particleSystem.Play();
            activeStats = new PlayerStats
                (armor: PlayerManager.PlayerStats.Armor * currentBonusArmor,
                passiveProcRate: currentBonusProcChance);
            PlayerManager.Instance.AddStats(activeStats);

            await UniTask.Delay(TimeSpan.FromSeconds(currentDuration), cancellationToken: cancellationToken).SuppressCancellationThrow();

            PlayerManager.Instance.AddStats(activeStats.Negated());
            activeStats = PlayerStats.Zero;
            particleSystem.Stop();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerManager.Instance.AddStats(activeStats.Negated());
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new ILevelField[]
            {
                Scriptable.Cooldown,
                duration.UseKey(LevelFieldKeys.EFFECT_DURATION).UseFormatter(StatFormatter.SECONDS),
                bonusArmor.UseKey(LevelFieldKeys.BONUS_ARMOR).UseFormatter(StatFormatter.PERCENT),
                bonusProcChance.UseKey(LevelFieldKeys.BONUS_PROC_CHANCE).UseFormatter(StatFormatter.PERCENT)
            };
        }
    }
}