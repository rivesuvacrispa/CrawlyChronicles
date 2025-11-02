using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Timeline;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Mutations.Active
{
    public class CellularRepair : ActiveAbility
    {
        [SerializeField] private int tickAmount;
        [SerializeField] private new ParticleSystem particleSystem;
        [Header("Health Per Tick")]
        [SerializeField] private float healthPerTickLvl1;
        [SerializeField] private float healthPerTickLvl10;
        [Header("Lifespan Per Tick")]
        [SerializeField] private float lifespanPerTickLvl1;
        [SerializeField] private float lifespanPerTickLvl10;

        private float lifespanPerTick;
        private float healthPerTick;
        


        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            healthPerTick = LerpLevel(healthPerTickLvl1, healthPerTickLvl10, lvl);
            lifespanPerTick = LerpLevel(lifespanPerTickLvl1, lifespanPerTickLvl10, lvl);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            RegenerationTask(
                    CancellationTokenSource.CreateLinkedTokenSource(
                        gameObject.GetCancellationTokenOnDestroy(),
                        MainMenu.CancellationTokenOnReset).Token
                    ).Forget();
        }

        private async UniTask RegenerationTask(CancellationToken cancellationToken)
        {
            particleSystem.Play();
            TimeSpan delay = TimeSpan.FromSeconds(1f);
            for (int i = 0; i < tickAmount; i++)
            {
                PlayerManager.Instance.AddHealth(healthPerTick);
                TimeManager.Instance.AddLifespan(lifespanPerTick);
                bool canceled = await UniTask.Delay(delay, cancellationToken: cancellationToken).SuppressCancellationThrow();
                if (canceled) break;
            }
            particleSystem.Stop();
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return null;
        }
    }
}