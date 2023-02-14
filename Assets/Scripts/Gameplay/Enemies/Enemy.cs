using System.Collections;
using Definitions;
using Pathfinding;
using UI;
using UnityEngine;
using Util;

namespace Gameplay.Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Scriptable.Enemy scriptable;
        [SerializeField] private EnemyBody body;


        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Rigidbody2D rb;
        private AIPath path;
        private AIDestinationSetter destinationSetter;

        private int walkHash;
        private int idleHash;
        private int deadhash;

        private HealthBar healthBar;
        private int health;

        public Scriptable.Enemy Scriptable => scriptable;

        

        public void SetTarget(Transform t) => destinationSetter.target = t;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            path = GetComponent<AIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();
            walkHash = Animator.StringToHash(scriptable.AnimatorName + "Walk");
            idleHash = Animator.StringToHash(scriptable.AnimatorName + "Idle");
            deadhash = Animator.StringToHash(scriptable.AnimatorName + "Dead");
            health = scriptable.MaxHealth;
            spriteRenderer.color = scriptable.Color;
        }

        private void Start()
        {
            healthBar = HealthbarPool.Instance.Create(this);
        }


        public void Damage(Vector2 fromPos, int damage, float knockbackPower)
        {
            health -= damage;
            ApplyKnockback(fromPos, knockbackPower);
            healthBar.SetValue(health, Mathf.Clamp01((float) health / scriptable.MaxHealth));
            if (health <= 0) Die();
            else StartCoroutine(ImmunityRoutine());
        }

        private void Die()
        {
            body.gameObject.SetActive(false);
            path.enabled = false;
            destinationSetter.target = null;
            rb.rotation = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            yield return new WaitForEndOfFrame();
            
            animator.Play(deadhash);
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
            path.enabled = false;
            body.enabled = false;

            float t = 0;
            while (t < scriptable.ImmunityDuration)
            {
                spriteRenderer.color = scriptable.GetImmunityFrameColor(t);
                t += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = scriptable.Color;
            path.enabled = true;
            body.enabled = true;
            animator.Play(walkHash);
        }

        private void ApplyKnockback(Vector2 from, float knockbackPower) => rb.velocity = (rb.position - from).normalized * knockbackPower;
    }
}