using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pooling;
using SoundEffects;
using UnityEngine;
using Util;

namespace Gameplay.Effects.WildCucumber
{
    public class WildCucumberProjectile : Poolable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private new Collider2D collider;
        [SerializeField] private BodyPainter painter;
        [SerializeField] private SimpleAudioSource popSource;
        [SerializeField] private SimpleAudioSource crackSource;
        
        private WildCucumberArguments args;
        private int healthLeft;
        private bool immune;

        
        
        private void OnCollisionEnter2D(Collision2D col) => Hit();

        private void OnCollisionStay2D(Collision2D collision) => Hit();

        private void Hit()
        {
            if (immune) return;
            
            healthLeft -= 1;
            if (healthLeft > 0)
            {
                ImmuneTask(gameObject.GetCancellationTokenOnDestroy(), 0.35f).Forget();
                painter.Paint(new Gradient().FastGradient(Color.white, spriteRenderer.color), 0.35f);
                crackSource.Play(pitch: SoundUtility.GetRandomPitchTwoSided(0.25f));
            } else ExplodeBody();
        }

        private async UniTask ImmuneTask(CancellationToken cancellationToken, float duration)
        {
            immune = true;
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
            immune = false;
        }

        private void ExplodeBody()
        {
            collider.enabled = false;
            rigidbody.simulated = false;
            spriteRenderer.enabled = false;
            particleSystem.Play();
            PoolTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
            popSource.Play(pitch: SoundUtility.GetRandomPitchTwoSided(0.2f));
        }

        private void ResetBody()
        {
            collider.enabled = true;
            rigidbody.simulated = true;
            spriteRenderer.enabled = true;
        }

        private async UniTask PoolTask(CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(() => !particleSystem.isPlaying, cancellationToken: cancellationToken);
            Pool();
        }
        
        public override bool OnTakenFromPool(object data)
        {
            if (data is not WildCucumberArguments arguments) return false;

            args = arguments;
            return base.OnTakenFromPool(data);
        }

        private void OnEnable()
        {
            healthLeft = 5;
            ResetBody();
        }
    }
}