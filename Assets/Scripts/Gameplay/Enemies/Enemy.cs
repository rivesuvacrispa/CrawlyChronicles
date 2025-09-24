using System.Collections;
using Definitions;
using GameCycle;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using Gameplay.Map;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Mutations.EntityEffects;
using Gameplay.Player;
using SoundEffects;
using Timeline;
using UI;
using UI.Elements;
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
    public abstract class Enemy : MonoBehaviour, IDamageableEnemy, IEnemyAttack, IImpactEffectAffectable
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
        public Scriptable.Enemy Scriptable => scriptable;
        public Vector2 Position => rb.position;
        public void SetMovementSpeed(float speed) => stateController.SpeedMultiplier = speed;

        
        
        public abstract void OnMapEntered();
        public abstract void OnPlayerLocated();

        public abstract void OnEggsLocated(EggBed eggBed);

        public abstract void OnFoodLocated(Foodbed foodBed);

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
            bool ignoreArmor = false,
            AttackEffect effect = null)
        {
            if (hitbox.Immune) return 0;

            if (damage is float.NaN or 0f)
            {
                damage = float.Epsilon;
            }

            if (!ignoreArmor && reckoned)
            {
                reckoned = false;
                attackDelay = 0.5f;
                return 0;
            }

            damage = ignoreArmor ? damage : PhysicsUtility.CalculateDamage(damage, Scriptable.Armor);
            StatRecorder.damageDealt += damage;
            Debug.Log($"{gameObject.name} damaged for {damage} HP, ignore armor: {ignoreArmor}");
            health -= damage;
            attackDelay = 1f;
            StopAttack();
            UpdateHealthbar();
            OnDamageTaken();

            if (health <= 0)
                Die();
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

            effect?.Impact(this, damage);

            return damage;
        }

        private void UpdateHealthbar()
        {
            healthbar.SetValue(Mathf.Clamp01(health / scriptable.MaxHealth));
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

        public void Die()
        {
            Debug.Log($"[{gameObject.name}] died, all coroutines are stopped");
            health = 0;
            hitbox.Die();
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
            Debug.Log($"[{gameObject.name}] attacked");
            stateController.TakeMoveControl();
            
            float t = attackDelay * 0.5f;
            while (t > 0)
            {
                if(!rb.freezeRotation) rb.RotateTowardsPosition(PlayerMovement.Position, 5);
                t -= Time.deltaTime;
                yield return null;
            }
            
            audioController.PlayAction(scriptable.AttackAudio);
            attackGO.SetActive(true);
            if(scriptable.HasAttackAnimation) animator.Play(scriptable.AttackAnimHash);
            var playerPos = PlayerMovement.Position;
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

        public void OnSpawnedBySpawner()
        {
            spawnedBySpawner = true;
            SpawnLocation = MapManager.GetRandomSpawnPoint();
            stateController.StartingState = AIState.Wander;
            if (TimeManager.IsDay) fearless = true;
        }
        
        
        
        // IImpactEffectAffectable
        public void AddEffect<T>(EntityEffectData data) where T : EntityEffect
        {
            if(!hitbox.Dead) effectController.AddEffect<T>(data);
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