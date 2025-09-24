using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Bosses.Terrorwing
{
    public class TerrorwingBulletHell : MonoBehaviour
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

        private void OnBulletCollision(IDamageable damageable)
        {
            if (damageable is Player.PlayerManager)
            {
                damageable.Damage(
                    TerrorwingDefinitions.BulletHellDamage, 
                    transform.position,
                    2,
                    0, Color.white, true);
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