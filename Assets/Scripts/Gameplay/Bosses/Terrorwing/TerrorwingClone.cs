using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Util;
using Util.Interfaces;

namespace Gameplay.Bosses.Terrorwing
{
    public class TerrorwingClone : MonoBehaviour, IDamageableEnemy
    {
        [SerializeField] private bool original;

        [Header("Colliders")]
        [SerializeField] private TerrorwingHitbox hitbox;

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
            hitbox.Die();
            bodyParticles1.Stop();
            bodyParticles2.Stop();
            SetTrailsActive(false);
            ResetColor(fromPlayer ? default : new Color(0, 0, 0, 0.01f));

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
        
        private void OnBulletCollision(IDamageable damageable)
        {
            if (damageable is PlayerManager)
            {
                damageable.Damage(
                    TerrorwingDefinitions.BulletHellDamage, 
                    transform.position,
                    2,
                    0, Color.white, true);
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
        public bool Immune => hitbox.Immune;
        public float Armor => 0;
        public float CurrentHealth { get; set; }

        public float Damage(
            float damage, 
            Vector3 position = default, 
            float knockback = 0f, 
            float stunDuration = 0f, 
            Color damageColor = default,
            bool piercing = false,
            AttackEffect effect = null)
        {
            if(hitbox.Immune) return 0;
            if(rb.simulated) hitbox.Hit();
            PaintDamage();
            if(original) ((IDamageable)terrorwing).Damage(
                damage, position, knockback, stunDuration, damageColor, piercing, effect);
            return damage;
        }

        public void OnLethalHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false, AttackEffect effect = null)
        {
            
        }

        public void OnHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false, AttackEffect effect = null)
        {
            
        }
    }
}