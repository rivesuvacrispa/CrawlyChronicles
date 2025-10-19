using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Mutations;
using Gameplay.Player;
using Pooling;
using SoundEffects;
using UnityEngine;
using Util;
using Util.Interfaces;
using Random = UnityEngine.Random;

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

        private int healthLeft;
        private bool immune;
        private ParticleSystem.Burst burst;

        private WildCucumberArguments args;

        

        private void Awake()
        {
            burst = particleSystem.emission.GetBurst(0);
        }

        private void OnCollisionEnter2D(Collision2D col) => Hit();

        private void OnCollisionStay2D(Collision2D collision) => Hit();

        private void Hit()
        {
            if (immune) return;
            
            healthLeft -= 1;

            if (healthLeft <= 0 || Random.value <= args.explosionChance)
            {
                HideBody();
                Explode();
                return;
            }

            StartImmune(0.35f);
            painter.Paint(new Gradient().FastGradient(Color.white, spriteRenderer.color), 0.35f);
            crackSource.Play(pitch: SoundUtility.GetRandomPitchTwoSided(0.25f));
            spriteRenderer.transform.DOShakePosition(0.35f, 0.05f, 50);
        }

        private void StartImmune(float duration)
        {
            ImmuneTask(gameObject.GetCancellationTokenOnDestroy(), duration).Forget();
        }

        private async UniTask ImmuneTask(CancellationToken cancellationToken, float duration)
        {
            immune = true;
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
            immune = false;
        }

        private void Explode()
        {
            particleSystem.Play();
            PoolTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
            popSource.Play(pitch: SoundUtility.GetRandomPitchTwoSided(0.2f));
            // TODO convert to cached results list
            var colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f,
                LayerMask.GetMask("EnemyPhysics", "PlayerPhysics"));
            foreach (Collider2D c in colliders)
            {
                if (c.Equals(PlayerManager.Instance.Collider)) continue;
                c.attachedRigidbody.AddForce( (c.transform.position - transform.position).normalized *
                                              args.knockback,ForceMode2D.Impulse );
            }
        }

        private void HideBody()
        {
            collider.enabled = false;
            rigidbody.simulated = false;
            spriteRenderer.enabled = false;
        }

        private void ShowBody()
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

            ShowBody();
            args = arguments;
            healthLeft = 5;
            burst.count = args.seedsAmount;
            particleSystem.emission.SetBurst(0, burst);
            SpawnTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
            
            return base.OnTakenFromPool(data);
        }
        
        private async UniTask SpawnTask(CancellationToken cancellationToken)
        {
            collider.enabled = false;
            immune = true;
            spriteRenderer.color = spriteRenderer.color.WithAlpha(0f);
            
            await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            rigidbody.AddForce(args.direction * args.projectilePower, ForceMode2D.Impulse);
            rigidbody.AddTorque((180 + Random.value * 180) * Random.value < 0.5f ? 1f : -1f, ForceMode2D.Impulse);

            await spriteRenderer.DOColor(spriteRenderer.color.WithAlpha(1f), 0.2f)
                .AsyncWaitForCompletion();
            immune = false;
            collider.enabled = true;
        }
        
        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out IDamageableEnemy enemy))
                enemy.Damage(
                    BasicAbility.GetAbilityDamage(args.seedsDamage),
                    transform.position,
                    0,
                    0,
                    Color.white,
                    false);
        }
    }
}