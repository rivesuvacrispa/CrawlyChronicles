using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using Gameplay.Map;
using Gameplay.Mutations.EntityEffects;
using Gameplay.Player;
using SoundEffects;
using Timeline;
using UI.Menus;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Util.Interfaces;

namespace Gameplay.Enemies
{
    [RequireComponent(typeof(Animator)),
     RequireComponent(typeof(Rigidbody2D)),
     RequireComponent(typeof(AIStateController)),
    RequireComponent(typeof(EffectController))]
    public abstract class Enemy : MonoBehaviour, IDamageableEnemy, IEffectAffectable
    {
        [FormerlySerializedAs("Fearless")] 
        [SerializeField] protected bool fearless;
        [SerializeField] private BodyPainter bodyPainter;
        [SerializeField] private GameObject attackGO;
        [SerializeField] private AudioController audioController;
        [SerializeField] protected SpriteRenderer minimapIcon;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected DamageableEnemyHitbox hitbox;
        [SerializeField] protected Scriptable.Enemy scriptable;

        protected Animator animator;
        protected Rigidbody2D rb;
        public AIStateController StateController { get; private set; }
        private EffectController effectController;

        protected bool spawnedBySpawner;
        private bool stunned;
        private float attackDelay;
        private bool reckoned;
        private bool isAttacking;
        private CancellationTokenSource attackCancellationTokenSource;
    
        [field:SerializeField] public EnemySpawnLocation SpawnLocation { get; set; }
        public Scriptable.Enemy Scriptable => scriptable;
        public Vector2 Position => rb.position;
        public void SetMovementSpeed(float speed) => StateController.SpeedMultiplier = speed;

        
        
        public abstract void OnMapEntered();
        public abstract void OnPlayerLocated();

        public abstract void OnEggsLocated(EggBed eggBed);

        public abstract void OnFoodLocated(Foodbed foodBed);

        protected abstract void DamageTaken();

        protected void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            StateController = GetComponent<AIStateController>();
            effectController = GetComponent<EffectController>();
        }
        
        protected virtual void Start()
        {
            rb.mass = scriptable.Mass;
            CurrentHealth = scriptable.MaxHealth;
            attackDelay = scriptable.AttackDelay;
            spriteRenderer.color = scriptable.BodyColor;
            PlayCrawl();
            SubEvents();
        }

        private void Knockback(Vector2 attacker, float force, float duration)
        {
            float kbResistance = PhysicsUtility.GetKnockbackResistance(scriptable.Mass);
            rb.AddClampedForceBackwards(attacker, force * (1 - kbResistance), ForceMode2D.Impulse);
            StartCoroutine(KnockbackRoutine(duration));
        }
        
        public void Reckon(Vector2 attacker, float force)
        {
            StopAttack();
            attackDelay = 0.75f;
            Knockback(attacker, force, 0.3f);
            reckoned = true;
            StartCoroutine(CancelReckon());
        }

        private IEnumerator CancelReckon()
        {
            yield return new WaitForSeconds(0.5f);
            reckoned = false;
        }

        public virtual void Die()
        {
            Debug.Log($"[{gameObject.name}] died, all coroutines are stopped");
            CurrentHealth = 0;
            hitbox.Die();
            ClearEffects();
            minimapIcon.enabled = false;
            StopAllCoroutines();
            attackGO.SetActive(false);
            StateController.SetState(AIState.None);
            spriteRenderer.sortingLayerName = "Ground";
            spriteRenderer.sortingOrder = 0;
            rb.rotation = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            audioController.PlayAction(scriptable.DeathAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
            audioController.StopState();
            animator.Play(scriptable.DeadAnimHash);
            GlobalDefinitions.CreateRandomGeneDrop(Position);
            StartCoroutine(DeathRoutine());
            UnsubEvents();
            OnDeath?.Invoke(this);
        }

        protected void StopAttack()
        {
            CancelAttack();
            StateController.ReturnMoveControl();
            attackGO.SetActive(false);
            isAttacking = false;
            if (scriptable.HasAttackAnimation) PlayCrawl();
        }
        
        private void BasicAttack()
        {
            if(isAttacking || stunned) return;
            attackCancellationTokenSource = new CancellationTokenSource();
            AttackTask(attackCancellationTokenSource.Token).Forget();
        }

        protected virtual void AttackPlayer(float reachDistance = 0)
        {
            if (reachDistance == 0) reachDistance = scriptable.AttackDistance;
            StateController.SetState(AIState.Follow, 
                onTargetReach: BasicAttack,
                reachDistance: reachDistance);
        } 
        
        
        
        // Routines & utils
        private async UniTask AttackTask(CancellationToken cancellationToken)
        {
            Debug.Log($"[{gameObject.name}] attacked");
            StateController.TakeMoveControl();
            isAttacking = true;

            float t = attackDelay * 0.5f;
            while (t > 0)
            {
                if(!rb.freezeRotation) rb.RotateTowardsPosition(PlayerMovement.Position, 5);
                t -= Time.deltaTime;
                await UniTask.Yield(cancellationToken: cancellationToken);
            }
            audioController.PlayAction(scriptable.AttackAudio);
            attackGO.SetActive(true);
            if(scriptable.HasAttackAnimation) animator.Play(scriptable.AttackAnimHash);
            
            await PerformAttack(cancellationToken);

            if(scriptable.HasAttackAnimation) PlayCrawl();
            attackGO.SetActive(false);
            StateController.ReturnMoveControl();
            
            await UniTask.Delay(TimeSpan.FromSeconds(scriptable.AttackCooldown), cancellationToken: cancellationToken);
            attackDelay = scriptable.AttackDelay;
            isAttacking = false;
        }

        protected virtual async UniTask PerformAttack(CancellationToken cancellationToken)
        {
            var playerPos = PlayerMovement.Position;
            if(!rb.freezeRotation) rb.RotateTowardsPosition(playerPos, 90);
            rb.AddClampedForceTowards(playerPos, scriptable.AttackPower, ForceMode2D.Impulse);

            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), cancellationToken: cancellationToken);
        }
        
        private IEnumerator DeathRoutine()
        {
            bodyPainter.Paint(GlobalDefinitions.DeathGradient, 1f);
            rb.mass = 0.001f;
            yield return new WaitForSeconds(1f);
            
            rb.simulated = false;
            yield return new WaitForSeconds(GlobalDefinitions.DespawnTime);
            
            bodyPainter.FadeOut(1f);
            yield return new WaitForSeconds(1f);
            
            Destroy(gameObject);
        }

        private IEnumerator StunRoutine(float duration)
        {
            PlayIdle();

            float t = duration;
            while (t > 0)
            {
                StateController.TakeMoveControl();
                stunned = true;
                t -= Time.deltaTime;
                yield return null;
            }
            
            StateController.ReturnMoveControl();
            PlayCrawl();
            stunned = false;
            OnStunEnd();
        }

        protected virtual void OnStunEnd()
        {
            if(TimeManager.IsDay) OnDayStart(0);
        }

        private void PlayIdle()
        {
            animator.Play(scriptable.IdleAnimHash);
            audioController.StopState();
        }

        protected void PlayCrawl()
        {
            animator.Play(scriptable.WalkAnimHash);
            audioController.PlayState(scriptable.CrawlAudio, true, scriptable.CrawlPitch);
        }
        
        private IEnumerator KnockbackRoutine(float duration)
        {
            float t = duration;
            while (t > 0)
            {
                StateController.TakeMoveControl();
                t -= Time.deltaTime;
                yield return null;
            }
            
            StateController.ReturnMoveControl();
        }

        private void CancelAttack()
        {
            attackCancellationTokenSource?.Cancel();
            attackCancellationTokenSource?.Dispose();
            attackCancellationTokenSource = null;
        }


        private void ClearEffects() => effectController.ClearAll();
        

        
        protected virtual void OnDestroy()
        {
            CancelAttack();
            OnProviderDestroy?.Invoke(this);
            MainMenu.OnResetRequested -= OnResetRequested;
            UnsubEvents();
        }

        protected virtual void OnDayStart(int day)
        {
            if(fearless || SpawnLocation is null) return;
            PlayCrawl();
            StopAttack();
            ClearEffects();
            StateController.ReturnMoveControl();
            StateController.SetEtherial(true);
            StateController.SetState(AIState.Flee);
        }
        
        private void SubEvents()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            TimeManager.OnDayStart += OnDayStart;
        }

        private void UnsubEvents() => TimeManager.OnDayStart -= OnDayStart;

        // TODO: REMOVE
        private void OnResetRequested()
        {
            // Destroy(gameObject);
        }

        public void OnSpawnedBySpawner()
        {
            spawnedBySpawner = true;
            SpawnLocation = MapManager.GetRandomSpawnPoint();
            StateController.StartingState = AIState.Wander;
            if (TimeManager.IsDay) fearless = true;
        }
        
        
        // IEffectAffectable
        public EffectController EffectController => effectController;
        public bool CanApplyEffect => !hitbox.Dead;
        
        

        // IDamageable
        public event IDamageable.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => scriptable.HealthbarOffsetY;
        public float HealthbarWidth => scriptable.HealthbarWidth;
        public event IDamageable.DeathEvent OnDeath;
        public event IDamageable.DamageEvent OnDamageTaken;
        public bool Immune => hitbox.Immune || StateController.Etherial;
        public float Armor => Scriptable.Armor;
        public float CurrentHealth { get; set; }
        public float MaxHealth => Scriptable.MaxHealth;

        public bool TryBlockDamage(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            if (piercing || !reckoned) return false;
            reckoned = false;
            attackDelay = 0.5f;
            return true;
        }

        public void OnBeforeHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            OnDamageTaken?.Invoke(this, damage);
            attackDelay = 1f;
            StopAttack();
            DamageTaken();
        }

        public void OnLethalHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            Die();
        }

        public void OnHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            hitbox.Hit();
            audioController.PlayAction(scriptable.HitAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
            bodyPainter.Paint(new Gradient().FastGradient(damageColor, scriptable.BodyColor), GlobalDefinitions.EnemyImmunityDuration);
            if (stunDuration > 0) 
                StartCoroutine(StunRoutine(stunDuration));
            if (knockback > 0)
            {
                float duration = Mathf.Lerp(Mathf.Clamp01(knockback), 0, 0.25f);
                Knockback(position, knockback, duration);
            }
        }
    }
}