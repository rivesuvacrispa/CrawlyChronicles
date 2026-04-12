using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameCycle;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class Leafcutter : BasicAbility, IDamageSource
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField, Range(0, 10)] private float baseDamage;
        [SerializeField, Range(0, 1)] private float damagePerFood;

        private static readonly TimeSpan ActivationDelay = TimeSpan.FromSeconds(0.15f);
        
        
        
        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(new DamageSource(this, collisionID),
                baseDamage + damagePerFood * StatRecorder.PlantsEaten);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            AttackController.OnAttackStart += OnAttackStart;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            AttackController.OnAttackStart -= OnAttackStart;
        }

        private void OnAttackStart()
        {
           ActivateTask(gameObject.CreateCommonCancellationToken()).Forget();
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            await UniTask.Delay(ActivationDelay, cancellationToken: cancellationToken);
            particleSystem.Play();
        }
    }
}