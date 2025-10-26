using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Bosses.Terrorwing
{
    public class TerrorwingBulletHell : MonoBehaviour, IDamageSource
    {
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private ParticleSystem waveParticles;
        [SerializeField] private ParticleSystem spiralParticles;
        [SerializeField] private ParticleCollisionProvider collisionProvider1;
        [SerializeField] private ParticleCollisionProvider collisionProvider2;
        [SerializeField] private ParticleCollisionProvider collisionProvider3;

        private void Awake()
        {
            collisionProvider1.OnCollision += OnBulletCollision;
            collisionProvider2.OnCollision += OnBulletCollision;
            collisionProvider3.OnCollision += OnBulletCollision;
        }

        private void OnDestroy()
        {
            collisionProvider1.OnCollision -= OnBulletCollision;
            collisionProvider2.OnCollision -= OnBulletCollision;
            collisionProvider3.OnCollision -= OnBulletCollision;
        }

        private void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            if (damageable is Player.PlayerManager)
            {
                damageable.Damage(new DamageInstance(new DamageSource(this, collisionID),
                    TerrorwingDefinitions.BulletHellDamage, 
                    transform.position,
                    knockback: 2,
                    piercing: true));
            }
        }

        public async UniTask StartBulletHell(int stages, float overallDuration, CancellationToken cancellationToken = default)
        {
            StopBulletHell();
            await BulletHellTask(stages, overallDuration, cancellationToken);
        }

        private async UniTask BulletHellTask(int stages, float overallDuration, CancellationToken cancellationToken = default)
        {
            float stageDuration = overallDuration / stages;
            rainParticles.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(stageDuration), cancellationToken: cancellationToken);
            waveParticles.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(stageDuration), cancellationToken: cancellationToken);
            rainParticles.Stop();
            spiralParticles.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(stageDuration), cancellationToken: cancellationToken);
            StopBulletHell();
        }

        public void StopBulletHell()
        {
            rainParticles.Stop();
            waveParticles.Stop();
            spiralParticles.Stop();
        }
    }
}