using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Timeline;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class CellularRepair : ActiveAbility
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private LevelConst tickAmount = new LevelConst(10);
        [SerializeField, MinMaxRange(0, 5)] private LevelFloat healthPerTick = new LevelFloat(new Vector2(0.5f, 2f));
        [SerializeField, MinMaxRange(0, 5)] private LevelFloat lifespanPerTick = new LevelFloat(new Vector2(1, 3));


        private float currentLifespanPerTick;
        private float currentHealthPerTick;
        


        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentHealthPerTick = healthPerTick.AtLvl(lvl);
            currentLifespanPerTick = lifespanPerTick.AtLvl(lvl);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            RegenerationTask(CreateCommonCancellationToken()).Forget();
        }

        private async UniTask RegenerationTask(CancellationToken cancellationToken)
        {
            particleSystem.Play();
            TimeSpan delay = TimeSpan.FromSeconds(1f);
            for (int i = 0; i < tickAmount.Value; i++)
            {
                PlayerManager.Instance.AddHealth(currentHealthPerTick);
                TimeManager.Instance.AddLifespan(currentLifespanPerTick);
                bool canceled = await UniTask.Delay(delay, cancellationToken: cancellationToken).SuppressCancellationThrow();
                if (canceled) break;
            }
            particleSystem.Stop();
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                tickAmount.UseKey(LevelFieldKeys.EFFECT_DURATION).UseFormatter(StatFormatter.SECONDS),
                healthPerTick.UseKey(LevelFieldKeys.TOTAL_HEALTH).UseFormatter(new StatFormatter(multiplier: tickAmount.Value)),
                lifespanPerTick.UseKey(LevelFieldKeys.TOTAL_LIFESPAN).UseFormatter(StatFormatter.SECONDS.WithMultiplier(tickAmount.Value)),
            };
        }
    }
}