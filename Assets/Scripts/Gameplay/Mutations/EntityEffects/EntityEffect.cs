using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.EntityEffects
{
    public abstract class EntityEffect : MonoBehaviour
    {
        protected EntityEffectData Data { get; private set; }
        protected int Duration { get; set; }
        protected IEffectAffectable Target { get; set; }

        private TimeSpan tickrate = TimeSpan.FromSeconds(0.25f);

        protected abstract void OnApplied();
        protected abstract void Tick();
        protected abstract void OnRemoved();
        protected int TickCounter { get; private set; }
        protected int refreshedOnTick;
        
        public EntityEffect SetTarget(IEffectAffectable target)
        {
            Target = target;
            return this;
        }
        
        public void Refresh(EntityEffectData data)
        {
            Data = data;
            Duration = data.Duration;
            enabled = true;
            refreshedOnTick = TickCounter;
        }

        public void Cancel()
        {
            enabled = false;
        }

        private async UniTask UpdateTask(CancellationToken cancellationToken)
        {
            await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            OnApplied();

            while (enabled)
            {
                await UniTask.Delay(tickrate, cancellationToken: cancellationToken);
                
                if (Duration == 0 && refreshedOnTick != TickCounter)
                {
                    enabled = false;
                }
                
                Tick();
                TickCounter++;
                Duration--;
            }
        }

        private void OnEnable()
        {
            UpdateTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private void OnDisable()
        {
            OnRemoved();
            TickCounter = 0;
        }
    }
}