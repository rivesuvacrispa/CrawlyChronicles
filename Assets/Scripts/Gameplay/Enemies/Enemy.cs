using System.Collections;
using Definitions;
using Gameplay.AI;
using Gameplay.Food;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Gameplay.Enemies
{
    [RequireComponent(typeof(SpriteRenderer)),
     RequireComponent(typeof(Animator)),
     RequireComponent(typeof(Rigidbody2D)),
     RequireComponent(typeof(AIStateController))]
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] private Scriptable.Enemy scriptable;
        [FormerlySerializedAs("body")] [SerializeField] private EnemyHitbox hitbox;
        
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Rigidbody2D rb;
        protected AIStateController stateController;
        
        private int walkHash;
        private int idleHash;
        private int deadhash;

        private HealthBar healthBar;
        private int health;

        public EnemySpawnLocation SpawnLocation { get; set; }
        public Scriptable.Enemy Scriptable => scriptable;
        public Vector2 Position => rb.position;

        
        
        public abstract void OnMapEntered();
        public abstract void OnPlayerLocated();

        public abstract void OnEggsLocated(EggBed eggBed);

        public abstract void OnFoodLocated(FoodBed foodBed);

        public abstract void OnDamageTaken();
        
        
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
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
            healthBar = HealthbarPool.Instance.Create(this);
        }
        
        public void Damage(Vector2 fromPos, int damage, float knockbackPower)
        {
            health -= damage;
            ApplyKnockback(fromPos, knockbackPower);
            healthBar.SetValue(health, Mathf.Clamp01((float) health / scriptable.MaxHealth));
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
            hitbox.gameObject.SetActive(false);
            stateController.SetState(AIState.None);
            rb.rotation = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            animator.Play(deadhash);
            StartCoroutine(DeathRoutine());
        }
        
        
        
        // Routines & utils
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
            stateController.SetState(AIState.None);
            hitbox.enabled = false;

            float t = 0;
            while (t < scriptable.ImmunityDuration)
            {
                spriteRenderer.color = scriptable.GetImmunityFrameColor(t);
                t += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = scriptable.BodyColor;
            stateController.SetState(AIState.Follow);
            hitbox.enabled = true;
            animator.Play(walkHash);
        }

        protected virtual void ApplyKnockback(Vector2 from, float knockbackPower) => rb.velocity = (rb.position - from).normalized * knockbackPower;
    }
}