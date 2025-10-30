using System;
using System.Collections.Generic;
using System.Threading;
using Camera;
using Cysharp.Threading.Tasks;
using Definitions;
using DG.Tweening;
using Gameplay.Mutations;
using Gameplay.Player;
using Hitboxes;
using Pooling;
using SoundEffects;
using Timeline;
using UI.Menus;
using UnityEngine;
using Util;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Effects.WildCucumber
{
    [RequireComponent(typeof(ParticleCollisionProvider))]
    public class WildCucumberProjectile : Poolable, IDamageSource
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private new Collider2D collider;
        [SerializeField] private BodyPainter painter;
        [SerializeField] private SimpleAudioSource popSource;
        [SerializeField] private SimpleAudioSource crackSource;

        private static readonly List<Collider2D> OverlapResults = new(32);
        private ParticleCollisionProvider provider;

        private int healthLeft;
        private bool immune;
        private ParticleSystem.Burst burst;
        private WildCucumberArguments args;
        private CancellationTokenSource dayCancellationSource;
        

        private void Awake()
        {
            burst = particleSystem.emission.GetBurst(0);
            provider = GetComponent<ParticleCollisionProvider>();
            provider.OnCollision += OnBulletCollision;
        }

        private void OnDestroy()
        {
            provider.OnCollision -= OnBulletCollision;
        }

        private void OnEnable()
        {
            TimeManager.OnDayStart += OnDayStart;
            MainMenu.OnResetRequested += OnResetRequested;
        }

        private void OnDisable()
        {
            TimeManager.OnDayStart -= OnDayStart;
            MainMenu.OnResetRequested -= OnResetRequested;
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

        public void Explode()
        {
            particleSystem.Play();
            PoolTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
            popSource.Play(pitch: SoundUtility.GetRandomPitchTwoSided(0.2f));

            int contacts = Physics2D.OverlapCircle(transform.position, args.explosionRange, GlobalDefinitions.EnemyAndPlayerPhysicsContactFilter, OverlapResults);

            for (var i = 0; i < Mathf.Min(contacts, 32); i++)
            {
                var c = OverlapResults[i];
                
                if (c.gameObject.layer == GlobalDefinitions.PlayerPhysicsLayer &&
                    c.Equals(PlayerManager.Instance.Collider))
                {
                    continue;
                }
                
                if (c.gameObject.layer == GlobalDefinitions.EnemyPhysicsLayer && 
                    c.TryGetComponent(out IDamageableEnemy enemy))
                {
                    enemy.Damage(new DamageInstance(
                        new DamageSource(this),
                        BasicAbility.GetAbilityDamage(args.explosionDamage), 
                        transform.position,
                        args.knockback));
                }
            }
            
            MainCamera.Instance.Shake(
                1 - Mathf.Clamp01(
                    Vector2.Distance(
                        transform.position, 
                        MainCamera.Instance.transform.position) 
                    / 5f));
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

            args = arguments;
            if (args.instant) HideBody();
            else ShowBody();
            healthLeft = 5;
            burst.count = args.seedsAmount;
            particleSystem.emission.SetBurst(0, burst);
            dayCancellationSource ??= new CancellationTokenSource();
            SpawnTask(gameObject.GetCancellationTokenOnDestroy()).AttachExternalCancellation(dayCancellationSource.Token).Forget();
            
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
        
        private void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(new DamageInstance(
                new DamageSource(this, collisionID),
                    BasicAbility.GetAbilityDamage(args.seedsDamage),
                    transform.position));
        }

        private void OnDayStart(int dayCounter)
        {
            immune = true;
            collider.enabled = false;
            DespawnTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
            dayCancellationSource?.Cancel();
            dayCancellationSource?.Dispose();
            dayCancellationSource = null;
        }

        private async UniTask DespawnTask(CancellationToken cancellationToken)
        {
            await spriteRenderer.DOColor(spriteRenderer.color.WithAlpha(0f), 2f)
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
            Pool();
        }
        
        private void OnResetRequested() {
            Pool();
        }
    }
}