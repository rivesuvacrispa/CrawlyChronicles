using System.Collections;
using Definitions;
using GameCycle;
using Gameplay.AI;
using Gameplay.Food;
using Timeline;
using UI;
using UnityEngine;
using Util;

namespace Gameplay.Enemies
{
    [RequireComponent(typeof(Animator)),
     RequireComponent(typeof(Rigidbody2D)),
     RequireComponent(typeof(AIStateController))]
    public abstract class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Scriptable.Enemy scriptable;
        [SerializeField] private EnemyHitbox hitbox;
        [SerializeField] private GameObject attackGO;
        
        private Animator animator;
        protected Rigidbody2D rb;
        protected AIStateController stateController;
        
        private int walkHash;
        private int idleHash;
        private int deadhash;

        private Healthbar healthbar;
        private float health;
        private Coroutine attackRoutine;
        private bool stunned;

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

            walkHash = Animator.StringToHash(scriptable.AnimatorName + "Walk");
            idleHash = Animator.StringToHash(scriptable.AnimatorName + "Idle");
            deadhash = Animator.StringToHash(scriptable.AnimatorName + "Dead");

        }
        
        protected virtual void Start()
        {
            health = scriptable.MaxHealth;
            spriteRenderer.color = scriptable.BodyColor;
            healthbar = HealthbarPool.Instance.Create(this);
            SubEvents();
        }
        
        public void Damage(float damage, float knockback, float stunDuration) => 
            Damage(Player.Movement.Position, damage, knockback, stunDuration);

        public void Damage(Vector2 attacker, float damage, float knockbackPower, float stunDuration)
        {
            StatRecorder.damageDealt += damage;
            health -= damage;
            StopAttack();
            healthbar.SetValue(Mathf.Clamp01(health / scriptable.MaxHealth));
            if (health <= float.Epsilon) Die();
            else
            {
                OnDamageTaken();
                StartCoroutine(ImmunityRoutine());
                if (stunDuration > float.Epsilon) 
                    StartCoroutine(StunRoutine(stunDuration));
                if (knockbackPower > 0)
                {
                    var velocity = PhysicsUtility.GetKnockbackVelocity(rb.position, attacker, knockbackPower);
                    StartCoroutine(KnockbackRoutine(velocity));
                }
            }
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
            animator.Play(deadhash);
            if(Random.value <= scriptable.GeneDropRate) 
                GlobalDefinitions.CreateRandomGeneDrop(Position);
            StartCoroutine(DeathRoutine());
            UnsubEvents();
        }

        private void StopAttack()
        {
            if(attackRoutine is not null) 
                StopCoroutine(attackRoutine);
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
            attackGO.SetActive(true);
            stateController.TakeMoveControl();
            var playerPos = Player.Movement.Position;
            rb.rotation = PhysicsUtility.RotateTowardsPosition(rb.position, rb.rotation, playerPos, 45);
            rb.velocity = PhysicsUtility.GetKnockbackVelocity(rb.position, playerPos, -15);
            
            yield return new WaitForSeconds(0.33f);

            attackGO.SetActive(false);
            stateController.ReturnMoveControl();
            yield return new WaitForSeconds(1.25f);
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
            animator.Play(idleHash);

            float t = duration;
            while (t > 0)
            {
                stateController.TakeMoveControl();
                stunned = true;
                t -= Time.deltaTime;
                yield return null;
            }
            
            stateController.ReturnMoveControl();
            animator.Play(walkHash);
            stunned = false;
            if(TimeManager.IsDay) OnDayStart(0);
        }
        
        private IEnumerator KnockbackRoutine(Vector2 velocity)
        {
            float t = 0.25f;
            while (t > 0)
            {
                stateController.TakeMoveControl();
                rb.velocity = velocity;
                t -= Time.deltaTime;
                yield return null;
            }
            
            stateController.ReturnMoveControl();
        }
        
        private IEnumerator ImmunityRoutine()
        {
            hitbox.Disable();

            float t = 0;
            while (t < scriptable.ImmunityDuration)
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
            OnDamageableDestroy?.Invoke();
            MainMenu.OnResetRequested -= OnResetRequested;
            UnsubEvents();
        }

        protected virtual void OnDayStart(int day)
        {
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


        // IDamageable
        public event IDamageable.DamageableEvent OnDamageableDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => scriptable.HealthbarOffsetY;
        public float HealthbarWidth => scriptable.HealthbarWidth;
    }
}