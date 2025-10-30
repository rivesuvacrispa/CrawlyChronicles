using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Enemies
{
    public class DamageableEnemyHitbox : MonoBehaviour, IDamageSource
    {
        [SerializeField] private Component damagableComponent;

        private IDamageableEnemy enemy;
        private readonly HashSet<DamageSource> blockedSources = new();

        public bool Dead { get; private set; }
        public bool ImmuneToSource(DamageSource source) => !isActiveAndEnabled || Dead || blockedSources.Contains(source);

        private void Awake()
        {
            if (damagableComponent is IDamageableEnemy e)
                enemy = e;
            else
            {
                Debug.LogError($"Component {damagableComponent.name} is not IDamageableEnemy");
                gameObject.SetActive(false);
            }
        }

        /***
         * EnemyHitbox x PlayerAttack
         * Player attack collides with enemy 
         * Enemy takes damage
         */
        private void OnCollisionEnter2D(Collision2D col)
        {
            enemy.Damage(PlayerAttack.CreateDamageInstance());
        }
        
        /***
         * EnemyHitbox x PlayerAttack (trigger)
         * Player attack triggers with enemy
         * Enemy takes damage
         */
        private void OnTriggerEnter2D(Collider2D col)
        {
            enemy.Damage(PlayerAttack.CreateDamageInstance());
        }
        
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Die()
        {
            Dead = true;
            Disable();
        }

        public void Hit(DamageInstance instance)
        {
            if (!blockedSources.Add(instance.source)) return;
            
            ImmunityTask(instance, gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask ImmunityTask(DamageInstance instance, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(GlobalDefinitions.EnemyImmunityDuration), cancellationToken: cancellationToken);
            blockedSources.Remove(instance.source);
        }


        private void OnValidate()
        {
            if (gameObject.TryGetComponent(out IDamageable _))
                Debug.LogWarning("DamageableEnemyHitbox shouldn't be used on the same object as its wearer. Move it to child gameobject instead");
        }
    }
}