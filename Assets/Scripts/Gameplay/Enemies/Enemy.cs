using System.Collections;
using Definitions;
using Gameplay.AI;
using Gameplay.Food;
using Genes;
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
        private int health;
        private Coroutine attackRoutine;

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
        }
        
        public void Damage(Vector2 attacker, int damage, float knockbackPower)
        {
            health -= damage;
            StopAttack();
            rb.velocity = PhysicsUtility.GetKnockbackVelocity(rb.position, attacker, knockbackPower);
            healthbar.SetValue(health, Mathf.Clamp01((float) health / scriptable.MaxHealth));
            if (health <= 0) Die();
            else
            {
                OnDamageTaken();
                StartCoroutine(ImmunityRoutine());
            }
        }

        private void Die()
        {
            StopAllCoroutines();
            attackGO.SetActive(false);
            hitbox.Disable();
            stateController.SetState(AIState.None);
            rb.rotation = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            animator.Play(deadhash);
            if(scriptable.GetGeneDrop(out GeneType geneType)) 
                GlobalDefinitions.CreateGeneDrop(Position, geneType);
            StartCoroutine(DeathRoutine());
        }

        private void StopAttack()
        {
            if(attackRoutine is not null) StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        
        protected void BasicAttack()
        {
            if(attackRoutine is not null) return;
            attackRoutine = StartCoroutine(AttackRoutine());
        }
        
        
        
        // Routines & utils
        private IEnumerator AttackRoutine()
        {
            attackGO.SetActive(true);
            stateController.TakeMoveControl();
            rb.velocity = PhysicsUtility.GetKnockbackVelocity(rb.position, Player.Movement.Position, -15);
            
            yield return new WaitForSeconds(0.33f);

            attackGO.SetActive(false);
            stateController.ReturnMoveControl();
            yield return new WaitForSeconds(0.66f);
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
        
        private IEnumerator ImmunityRoutine()
        {
            animator.Play(idleHash);
            stateController.TakeMoveControl();
            hitbox.Disable();

            float t = 0;
            while (t < scriptable.ImmunityDuration)
            {
                spriteRenderer.color = scriptable.GetImmunityFrameColor(t);
                t += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = scriptable.BodyColor;
            stateController.ReturnMoveControl();
            hitbox.Enable();
            animator.Play(walkHash);
        }

        private void OnDestroy() => OnDamageableDestroy?.Invoke();


        // IDamageable
        public event IDamageable.DamageableEvent OnDamageableDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => scriptable.HealthbarOffsetY;
        public float HealthbarWidth => scriptable.HealthbarWidth;
    }
}