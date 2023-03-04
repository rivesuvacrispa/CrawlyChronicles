using System.Collections;
using Definitions;
using GameCycle;
using Gameplay.Abilities.EntityEffects;
using Gameplay.AI;
using Gameplay.Food;
using Scripts.SoundEffects;
using Timeline;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Enemies
{
    [RequireComponent(typeof(Animator)),
     RequireComponent(typeof(Rigidbody2D)),
     RequireComponent(typeof(AIStateController)),
    RequireComponent(typeof(EffectController))]
    public abstract class Enemy : MonoBehaviour, IDamageableEnemy, IEnemyAttack
    {
        [FormerlySerializedAs("Fearless")] 
        [SerializeField] protected bool fearless;
        [SerializeField] private BodyPainter bodyPainter;
        [SerializeField] private GameObject attackGO;
        [SerializeField] private AudioController audioController;
        [SerializeField] protected SpriteRenderer minimapIcon;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected EnemyHitbox hitbox;
        [SerializeField] protected Scriptable.Enemy scriptable;

        protected Animator animator;
        protected Rigidbody2D rb;
        protected AIStateController stateController;
        private Healthbar healthbar;
        private Coroutine attackRoutine;
        private EffectController effectController;

        protected bool spawnedBySpawner;
        private float health;
        private bool stunned;
        private float attackDelay;
        private bool reckoned;

        [field:SerializeField] public EnemySpawnLocation SpawnLocation { get; set; }
        public EnemyWallCollider WallCollider { get; set; }
        public Scriptable.Enemy Scriptable => scriptable;
        public Vector2 Position => rb.position;
        public void SetMovementSpeed(float speed) => stateController.SpeedMultiplier = speed;

        
        
        public abstract void OnMapEntered();
        public abstract void OnPlayerLocated();

        public abstract void OnEggsLocated(EggBed eggBed);

        public abstract void OnFoodLocated(FoodBed foodBed);

        protected abstract void OnDamageTaken();
        
        protected void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            stateController = GetComponent<AIStateController>();
            effectController = GetComponent<EffectController>();
        }
        
        protected virtual void Start()
        {
            rb.mass = scriptable.Mass;
            health = scriptable.MaxHealth;
            attackDelay = scriptable.AttackDelay;
            spriteRenderer.color = scriptable.BodyColor;
            healthbar = HealthbarPool.Instance.Create(this);
            PlayCrawl();
            SubEvents();
        }

        public float Damage(
            float damage, 
            Vector3 position,
            float knockback, 
            float stunDuration, 
            Color damageColor,
            bool ignoreArmor = false)
        {
            if (!hitbox.Enabled) return 0;
            
            if (!ignoreArmor && reckoned)
            {
                reckoned = false;
                attackDelay = 0.5f;
                return 0;
            }
        
            damage = ignoreArmor ? damage : PhysicsUtility.CalculateDamage(damage, Scriptable.Armor);
            StatRecorder.damageDealt += damage;
            health -= damage;
            attackDelay = 1f;
            StopAttack();
            healthbar.SetValue(Mathf.Clamp01(health / scriptable.MaxHealth));
            OnDamageTaken();

            if (health <= float.Epsilon)
            {
                Die();
                hitbox.Die();
            }
            else
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

            return damage;
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
            attackDelay = 0.5f;
            Knockback(attacker, force, 0.3f);
            reckoned = true;
            StartCoroutine(CancelReckon());
        }

        private IEnumerator CancelReckon()
        {
            yield return new WaitForSeconds(0.5f);
            reckoned = false;
        }

        private void Die()
        {
            ClearEffects();
            minimapIcon.enabled = false;
            StatRecorder.enemyKills++;
            StopAllCoroutines();
            attackGO.SetActive(false);
            stateController.SetState(AIState.None);
            spriteRenderer.sortingLayerName = "Ground";
            spriteRenderer.sortingOrder = 0;
            rb.rotation = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            audioController.PlayAction(scriptable.DeathAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
            audioController.StopState();
            animator.Play(scriptable.DeadAnimHash);
            if(Random.value <= Gamemode.GeneDropRate) 
                GlobalDefinitions.CreateRandomGeneDrop(Position);
            StartCoroutine(DeathRoutine());
            UnsubEvents();
        }

        protected void StopAttack()
        {
            if(attackRoutine is not null) 
                StopCoroutine(attackRoutine);
            stateController.ReturnMoveControl();
            attackRoutine = null;
            attackGO.SetActive(false);
            if(scriptable.HasAttackAnimation) PlayCrawl();
        }
        
        private void BasicAttack()
        {
            if(attackRoutine is not null || stunned) return;
            attackRoutine = StartCoroutine(AttackRoutine());
        }

        protected virtual void AttackPlayer(float reachDistance = 0)
        {
            if (reachDistance == 0) reachDistance = scriptable.AttackDistance;
            stateController.SetState(AIState.Follow, 
                onTargetReach: BasicAttack,
                reachDistance: reachDistance);
        } 
        
        
        
        // Routines & utils
        private IEnumerator AttackRoutine()
        {
            stateController.TakeMoveControl();
            
            float t = attackDelay * 0.5f;
            while (t > 0)
            {
                if(!rb.freezeRotation) rb.RotateTowardsPosition(Player.Movement.Position, 5);
                t -= Time.deltaTime;
                yield return null;
            }
            
            audioController.PlayAction(scriptable.AttackAudio);
            attackGO.SetActive(true);
            if(scriptable.HasAttackAnimation) animator.Play(scriptable.AttackAnimHash);
            var playerPos = Player.Movement.Position;
            if(!rb.freezeRotation) rb.RotateTowardsPosition(playerPos, 90);
            rb.AddClampedForceTowards(playerPos, scriptable.AttackPower, ForceMode2D.Impulse);
            
            yield return new WaitForSeconds(0.4f);

            if(scriptable.HasAttackAnimation) PlayCrawl();
            attackGO.SetActive(false);
            stateController.ReturnMoveControl();
            yield return new WaitForSeconds(attackDelay * 0.5f);
            attackDelay = scriptable.AttackDelay;
            attackRoutine = null;
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
                stateController.TakeMoveControl();
                stunned = true;
                t -= Time.deltaTime;
                yield return null;
            }
            
            stateController.ReturnMoveControl();
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
                stateController.TakeMoveControl();
                t -= Time.deltaTime;
                yield return null;
            }
            
            stateController.ReturnMoveControl();
        }

        public void AddEffect<T>(EntityEffectData data) where T : EntityEffect
            => effectController.AddEffect<T>(data);

        private void ClearEffects() => effectController.ClearAll();
        

        
        protected virtual void OnDestroy()
        {
            OnProviderDestroy?.Invoke();
            MainMenu.OnResetRequested -= OnResetRequested;
            UnsubEvents();
        }

        protected virtual void OnDayStart(int day)
        {
            if(fearless || SpawnLocation is null) return;
            PlayCrawl();
            StopAttack();
            ClearEffects();
            stateController.ReturnMoveControl();
            stateController.SetEtherial(true);
            stateController.SetState(AIState.Flee);
        }
        
        private void SubEvents()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            TimeManager.OnDayStart += OnDayStart;
        }

        private void UnsubEvents() => TimeManager.OnDayStart -= OnDayStart;

        private void OnResetRequested() => Destroy(gameObject);

        public virtual void OnWallCollision() { }

        public void OnSpawnedBySpawner()
        {
            spawnedBySpawner = true;
            SpawnLocation = EnemyWaveSpawner.GetRandomSpawnPoint();
            stateController.StartingState = AIState.Wander;
            if (TimeManager.IsDay) fearless = true;
        }
        


        // IDamageable
        public event IDamageable.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => scriptable.HealthbarOffsetY;
        public float HealthbarWidth => scriptable.HealthbarWidth;
        
        // IEnemyAttack
        public Vector3 AttackPosition => transform.position;
        public float AttackDamage => scriptable.Damage;
        public float AttackPower => scriptable.AttackPower;
    }
}