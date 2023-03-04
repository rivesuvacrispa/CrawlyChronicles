using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scripts.Gameplay.Bosses.Terrorwing
{
    public class TerrorwingBulletHell : MonoBehaviour
    {
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private ParticleSystem waveParticles;
        [SerializeField] private ParticleSystem spiralParticles;

        public async UniTask StartBulletHell(int stages, float overallDuration, CancellationToken cancellationToken = default)
        {
            StopBulletHell();
            await BulletHellTask(stages, overallDuration, cancellationToken);
        }

        private async UniTask BulletHellTask(int stages, float overallDuration, CancellationToken cancellationToken = default)
        {
            float stageDuration = overallDuration /stages;
            
            rainParticles.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(stageDuration), cancellationToken: cancellationToken);
            if (stages < 2)
            {
                StopBulletHell();
                return;
            }
            
            waveParticles.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(stageDuration), cancellationToken: cancellationToken);
            if(stages < 3) 
            {
                StopBulletHell();
                return;
            }
            
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