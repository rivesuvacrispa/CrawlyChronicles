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
        private float DurationInSeconds { get; set; }
        protected IEffectAffectable Target { get; set; }

        protected const float TICKRATE = 0.25f;
        protected const float TICKS_PER_SECOND = 1f / TICKRATE;

        protected abstract void OnApplied();
        protected abstract void Tick();
        protected abstract void OnRemoved();
        protected virtual void OnRefreshed(EntityEffectData data){ }
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
            DurationInSeconds = data.DurationInSeconds;
            enabled = true;
            refreshedOnTick = TickCounter;
            OnRefreshed(data);
        }

        public void Cancel()
        {
            enabled = false;
        }

        private async UniTask UpdateTask(CancellationToken cancellationToken)
        {
            await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            OnApplied();

            TimeSpan delay = TimeSpan.FromSeconds(TICKRATE);
            while (enabled)
            {
                await UniTask.Delay(delay, cancellationToken: cancellationToken);
                
                if (DurationInSeconds <= 0 && refreshedOnTick != TickCounter)
                {
                    Cancel();
                    break;
                }

                Tick();
                TickCounter++;
                DurationInSeconds -= TICKRATE;
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