using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using Gameplay.Enemies;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Util;
using Util.Interfaces;

namespace Gameplay.Bosses.Terrorwing
{
    public class TerrorwingClone : MonoBehaviour, IDamageableEnemy, IDamageSource, IContactDamageProvider
    {
        [SerializeField] private bool original;

        [Header("Colliders")]
        [SerializeField] private DamageableEnemyHitbox hitbox;

        [Header("Refs")] 
        [SerializeField] private ParticleCollisionProvider particleCollisionProvider;
        [SerializeField] private Terrorwing terrorwing;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private ParticleSystem radialParticals;
        [SerializeField] private TrailRenderer[] trails = new TrailRenderer[4];
        [SerializeField] private BodyPainter[] painters = new BodyPainter[5];
        [SerializeField] private TerrorwingProjectileSpawner[] projectileSpawners;

        [Header("Disable upon death")] 
        [SerializeField] private new Light2D light;
        [SerializeField] private ParticleSystem bodyParticles1;
        [SerializeField] private ParticleSystem bodyParticles2;
        
        private bool alive;


        public TerrorwingProjectileSpawner[] ProjectileSpawners => projectileSpawners;
        public void SetSimulated(bool isSimulated)
        {
            if (!isSimulated) rb.rotation = 0;
            rb.simulated = isSimulated;
        }

        private void Start()
        {
            particleCollisionProvider.OnCollision += OnBulletCollision;
            if (original) alive = true;
        }

        public async UniTask SetActive(bool isActive, float fadeDuration, CancellationToken cancellationToken = default)
        {
            if(alive != isActive)
            {
                gameObject.SetActive(true);
                SetTrailsActive(false);
                
                if (isActive)
                {
                    if (original) fadeDuration *= 0.75f;
                    foreach (BodyPainter painter in painters) painter.FadeIn(fadeDuration);
                    alive = true;
                    await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);
                    SetTrailsActive(true);
                }
                else
                {
                    if (original) fadeDuration *= 1.25f;
                    foreach (BodyPainter painter in painters) painter.FadeOut(fadeDuration);
                    alive = false;
                    await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);
                    gameObject.SetActive(false);
                }
            }
        }

        public void ResetColor(Color c = default)
        {
            foreach (BodyPainter painter in painters) 
                painter.ResetColor(c);
        }
        
        private void SetTrailsActive(bool isActive)
        {
            foreach (TrailRenderer trail in trails) trail.emitting = isActive;
        }

        public void ShootRadial() => radialParticals.Play();

        public async UniTask ShootBullets(TerrorwingProjectile projectile, int amount, float delay, CancellationToken cancellationToken = default)
        {
            for(int i = 0; i < amount; i++)
            {
                projectileSpawners[UnityEngine.Random.Range(0, 4)].Spawn(projectile);
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            }
        }

        public void UpdateIllusionPosition(Vector2 pos, Vector2 playerPos)
        {
            transform.localPosition = pos;
            rb.RotateTowardsPosition(playerPos, 360);
        }

        public void DieIfClone()
        {
            if (original || !gameObject.activeInHierarchy) return;
            
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
        
        public async UniTask Die(CancellationToken cancellationToken = default, bool fromPlayer = true)
        {
            bodyParticles1.Stop();
            bodyParticles2.Stop();
            SetTrailsActive(false);
            ResetColor(fromPlayer ? default : new Color(0, 0, 0, 0.01f));
            OnDeath?.Invoke(this);

            if(fromPlayer)
            {
                foreach (BodyPainter painter in painters)
                    painter.PlayDead(100);
                await UniTask.Delay(TimeSpan.FromSeconds(6f), cancellationToken: cancellationToken);
                await SetActive(false, 2f, cancellationToken: cancellationToken);
            }
        }

        public void PaintDamage()
        {
            foreach (BodyPainter painter in painters)
                painter.Paint(new Gradient().FastGradient(Color.black, Color.white),
                    GlobalDefinitions.EnemyImmunityDuration);
        }

        private void OnDestroy()
        {
            OnProviderDestroy?.Invoke(this);
            OnDeath?.Invoke(this);
            particleCollisionProvider.OnCollision -= OnBulletCollision;
        }
        
        private void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            if (damageable is PlayerManager)
            {
                damageable.Damage(new DamageInstance(new DamageSource(this, collisionID),
                    TerrorwingDefinitions.BulletHellDamage,
                    transform.position,
                    knockback: 2,
                    piercing: true));
            }
        }


        // Used by animator as an event
        private void DisableAnimator()
        {
            GetComponent<Animator>().enabled = false;
            light.enabled = false;
        }
        
        
        
        // IDamageableEnemy
        public event IDestructionEventProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => 0;
        public float HealthbarWidth => 0;
        public event IDamageable.DeathEvent OnDeath;
        public event IDamageable.DamageEvent OnDamageTaken;
        public float Armor => 0;
        public float CurrentHealth { get; set; }

        public float MaxHealth => TerrorwingDefinitions.MaxHealth;
        public IDamageableHitbox Hitbox => hitbox;

        public void OnBeforeHit(DamageInstance instance)
        {
            OnDamageTaken?.Invoke(this, instance.Damage);
        }
        

        public void OnLethalHit(DamageInstance instance)
        {
            
        }

        public void OnHit(DamageInstance instance)
        {
            // if(rb.simulated) hitbox.Hit(instance);
            PaintDamage();
            if(original) ((IDamageable)terrorwing).Damage(
                new DamageInstance(new DamageSource(this), 
                    instance.Damage, 
                    instance.position, 
                    instance.knockback, 
                    instance.stunDuration, 
                    instance.damageColor, 
                    instance.piercing, 
                    instance.effects)
            );
        }

        // I ContactDamageProvider
        public float ContactDamage => TerrorwingDefinitions.ContactDamage;
        public Vector3 ContactDamagePosition => transform.position;
        public float ContactDamageKnockback => 5;
        public float ContactDamageStunDuration => 0;
        public Color ContactDamageColor => default;
        public bool ContactDamagePiercing => true;
    }
}