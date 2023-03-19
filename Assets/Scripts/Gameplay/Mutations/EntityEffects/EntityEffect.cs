using System.Collections;
using Scripts.Util.Interfaces;
using UnityEngine;

namespace Gameplay.Abilities.EntityEffects
{
    public abstract class EntityEffect : MonoBehaviour
    {
        protected EntityEffectData Data { get; private set; }
        protected int Duration { get; set; }
        protected IImpactEffectAffectable Target { get; set; }
        
        private Coroutine tickRoutine;


        protected abstract void OnApplied();
        protected abstract void Tick();
        protected abstract void OnRemoved();

        public EntityEffect SetTarget(IImpactEffectAffectable target)
        {
            Target = target;
            return this;
        }
        
        public void Refresh(EntityEffectData data)
        {
            Data = data;
            Duration = data.Duration;
            if (tickRoutine is null)
            {
                tickRoutine = StartCoroutine(TickRoutine());
                OnApplied();
            }
        }
        
        private IEnumerator TickRoutine()
        {
            OnApplied();
            
            while (Duration > 0)
            {
                yield return new WaitForSeconds(1f);
                Duration--;
                Tick();
            }

            enabled = false;
            tickRoutine = null;
            OnRemoved();
        }

        public void Cancel()
        {
            enabled = false;
            if (tickRoutine is null) return;
            StopCoroutine(tickRoutine);
            tickRoutine = null;
            OnRemoved();
        }
    }
}