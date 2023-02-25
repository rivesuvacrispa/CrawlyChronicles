using System;
using System.Collections;
using Definitions;
using GameCycle;
using Gameplay.AI;
using Gameplay.Food;
using Scripts.SoundEffects;
using Timeline;
using UI;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Enemies
{
    [RequireComponent(typeof(Animator)),
     RequireComponent(typeof(Rigidbody2D)),
     RequireComponent(typeof(AIStateController))]
    public abstract class Enemy : MonoBehaviour, IDamageable
    {
        public bool debug_Fearless;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject attackGO;
        [SerializeField] protected EnemyHitbox hitbox;
        [SerializeField] protected Scriptable.Enemy scriptable;
        [SerializeField] private AudioController audioController;

        protected Animator animator;
        protected Rigidbody2D rb;
        protected AIStateController stateController;
        
        private Healthbar healthbar;
        private float health;
        private Coroutine attackRoutine;
        private bool stunned;
        private float attackDelay = 0.75f;
        private bool reckoned;

        [field:SerializeField] public EnemySpawnLocation SpawnLocation { get; set; }
        public Scriptable.Enemy Scriptable => scriptable;
        public Vector2 Position => rb.position;

        
        
        public abstract void OnMapEntered();
        public abstract void OnPlayerLocated();

        public abstract void OnEggsLocated(EggBed eggBed);

        public abstract void OnFoodLocated(FoodBed foodBed);

        protected abstract void OnDamageTaken();
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            stateController = GetComponent<AIStateController>();
        }
        
        protected virtual void Start()
        {
            rb.mass = scriptable.Mass;
            health = scriptable.MaxHealth;
            spriteRenderer.color = scriptable.BodyColor;
            healthbar = HealthbarPool.Instance.Create(this);
            PlayCrawl();
            SubEvents();
        }
        
        public void Damage(float damage, float knockback, float stunDuration) => 
            Damage(Player.Movement.Position, damage, knockback, stunDuration);

        private void Damage(Vector2 attacker, float damage, float knockbackPower, float stunDuration)
        {
            if (reckoned)
            {
                reckoned = false;
                attackDelay = 0.5f;
                return;
            }
        
            damage = PhysicsUtility.CalculateDamage(damage, Scriptable.Armor);
            StatRecorder.damageDealt += damage;
            health -= damage;
            attackDelay = 1f;
            StopAttack();
            healthbar.SetValue(Mathf.Clamp01(health / scriptable.MaxHealth));
            OnDamageTaken();

            if (health <= float.Epsilon) Die();
            else
            {
                audioController.PlayAction(scriptable.HitAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
                StartCoroutine(ImmunityRoutine());
                if (stunDuration > 0) 
                    StartCoroutine(StunRoutine(stunDuration));
                if (knockbackPower > 0) 
                    Knockback(attacker, knockbackPower);
            }
        }

        private void Knockback(Vector2 attacker, float force)
        {
            float kbResistance = PhysicsUtility.GetKnockbackResistance(scriptable.Mass);
            rb.AddClampedForceBackwards(attacker, force * (1 - kbResistance), ForceMode2D.Impulse);
            StartCoroutine(KnockbackRoutine());
        }

        public void Reckon(Vector2 attacker, float force)
        {
            StopAttack();
            attackDelay = 0.5f;
            Knockback(attacker, force);
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
            StatRecorder.enemyKills++;
            StopAllCoroutines();
            attackGO.SetActive(false);
            hitbox.Disable();
            stateController.SetState(AIState.None);
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
        
        private void StopAttack()
        {
            if(attackRoutine is not null) 
                StopCoroutine(attackRoutine);
            stateController.ReturnMoveControl();
            attackRoutine = null;
            attackGO.SetActive(false);
        }
        
        protected void BasicAttack()
        {
            if(attackRoutine is not null || stunned) return;
            attackRoutine = StartCoroutine(AttackRoutine());
        }
        
        
        
        // Routines & utils
        private IEnumerator AttackRoutine()
        {
            yield return new WaitForSeconds(attackDelay);
            audioController.PlayAction(scriptable.AttackAudio);
            attackGO.SetActive(true);
            stateController.TakeMoveControl();
            var playerPos = Player.Movement.Position;
            rb.RotateTowardsPosition(playerPos, 90);
            rb.AddClampedForceTowards(playerPos, scriptable.AttackPower, ForceMode2D.Impulse);
            
            yield return new WaitForSeconds(0.4f);

            attackGO.SetActive(false);
            stateController.ReturnMoveControl();
            attackDelay = 0.75f;
            attackRoutine = null;
        }
        
        private IEnumerator DeathRoutine()
        {
            float t = 0;
            while (t < 1f)
            {
                spriteRenderer.color = GlobalDefinitions.GetDeadColor(t);
                t += Time.deltaTime;
                yield return null;
            }
            
            rb.simulated = false;
            Color deadColor = GlobalDefinitions.GetDeadColor(1);
            spriteRenderer.color = deadColor;

            yield return new WaitForSeconds(GlobalDefinitions.DespawnTime);
            
            t = 0;
            while (t < 1f)
            {
                spriteRenderer.color = deadColor.WithAlpha(1 - t);
                t += Time.deltaTime;
                yield return null;
            }
            
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

        private void PlayCrawl()
        {
            animator.Play(scriptable.WalkAnimHash);
            audioController.PlayState(scriptable.CrawlAudio, true, scriptable.CrawlPitch);
        }
        
        private IEnumerator KnockbackRoutine()
        {
            float t = 0.25f;
            while (t > 0)
            {
                stateController.TakeMoveControl();
                t -= Time.deltaTime;
                yield return null;
            }
            
            stateController.ReturnMoveControl();
        }
        
        private IEnumerator ImmunityRoutine()
        {
            hitbox.Disable();

            float t = 0;
            while (t < GlobalDefinitions.EnemyImmunityDuration)
            {
                spriteRenderer.color = scriptable.GetImmunityFrameColor(t);
                t += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = scriptable.BodyColor;
            hitbox.Enable();
        }
        
        protected virtual void OnDestroy()
        {
            OnProviderDestroy?.Invoke();
            MainMenu.OnResetRequested -= OnResetRequested;
            UnsubEvents();
        }

        protected virtual void OnDayStart(int day)
        {
            if(debug_Fearless) return;
            stateController.SetEtherial(true);
            stateController.SetState(AIState.Flee);
        }
        
        private void SubEvents()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            TimeManager.OnDayStart += OnDayStart;
        }

        private void UnsubEvents() => TimeManager.OnDayStart -= OnDayStart;

        private void OnResetRequested()
        {
            if(debug_Fearless) return;
            Destroy(gameObject);
        }


        // IDamageable
        public event IDamageable.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => scriptable.HealthbarOffsetY;
        public float HealthbarWidth => scriptable.HealthbarWidth;
    }
}