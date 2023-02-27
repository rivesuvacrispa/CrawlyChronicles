using System.Collections;
using Gameplay.Enemies;
using UnityEngine;

namespace Gameplay.Abilities.EntityEffects
{
    public abstract class EntityEffect : MonoBehaviour
    {
        protected EntityEffectData Data { get; set; }
        protected int Duration { get; set; }
        protected Enemy Enemy { get; set; }
        
        private Coroutine tickRoutine;


        protected abstract void OnApplied();
        protected abstract void Tick();
        protected abstract void OnRemoved();

        public EntityEffect Init(Enemy enemy)
        {
            Enemy = enemy;
            enemy.OnProviderDestroy += OnTargetDestroy;
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

        private void OnTargetDestroy()
        {
            if(tickRoutine is not null) StopCoroutine(tickRoutine);
            Enemy.OnProviderDestroy -= OnTargetDestroy;
            Destroy(this);
        }

        private void OnDestroy() => Enemy.OnProviderDestroy -= OnTargetDestroy;
    }
}