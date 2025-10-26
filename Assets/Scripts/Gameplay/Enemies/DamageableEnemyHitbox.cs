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
    [RequireComponent(typeof(Collider2D))]
    public class DamageableEnemyHitbox : MonoBehaviour
    {
        [SerializeField] private Component damagableComponent;

        private new Collider2D collider;
        private IDamageableEnemy enemy;
        private readonly HashSet<DamageSource> blockedSources = new();

        public bool Dead { get; private set; }
        public bool ImmuneToSource(DamageSource source) => !collider.enabled || Dead || blockedSources.Contains(source);

        private void Awake()
        {
            collider = GetComponent<Collider2D>();
            if (damagableComponent is IDamageableEnemy e)
                enemy = e;
            else
            {
                Debug.LogError($"Component {damagableComponent.name} is not IDamageableEnemy");
                enabled = false;
            }
        }

        /***
         * EnemyHitbox x PlayerAttack
         * Player attack collides with enemy
         * Enemy takes damage
         */
        private void OnCollisionEnter2D(Collision2D _)
        {
            enemy.Damage(PlayerAttack.CreateDamageInstance());
        }
        
        public void Enable()
        {
            StopAllCoroutines();
            collider.enabled = true;
        }

        public void Disable()
        {
            StopAllCoroutines();
            collider.enabled = false;
        }

        public void Die()
        {
            Dead = true;
            StopAllCoroutines();
            Disable();
        }

        public void Hit(DamageInstance instance)
        {
            ImmunityTask(instance, gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask ImmunityTask(DamageInstance instance, CancellationToken cancellationToken)
        {
            if (!blockedSources.Add(instance.source)) return;
            
            await UniTask.Delay(TimeSpan.FromSeconds(GlobalDefinitions.EnemyImmunityDuration), cancellationToken: cancellationToken);

            blockedSources.Remove(instance.source);
        }
    }
}