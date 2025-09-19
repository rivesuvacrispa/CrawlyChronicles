using System.Collections;
using Definitions;
using GameCycle;
using Gameplay.AI.Locators;
using Mutations.AttackEffects;
using Player;
using Scripts.Util.Interfaces;
using UI;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Scripts.Gameplay.Bosses.Centipede
{
    [RequireComponent(typeof(BodyPainter))]
    public class CentipedeFragment : MonoBehaviour, IDamageableEnemy, IImpactable
    {
        [SerializeField] private ParticleSystem sprayParticles;
        [SerializeField] private ParticleCollisionProvider particleCollisionProvider;
        [SerializeField] private GameObject attackGO;
        [SerializeField] private Locator locator;

        private Animator animator;
        private float currentHealth;
        private CentipedeFragment frontFragment;
        private CentipedeFragment backFragment;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;
        private Healthbar healthbar;
        private BodyPainter painter;
        private CentipedeHitbox hitbox;

        private Coroutine sprayCooldownRoutine;
        private bool dead;
        private CentipedeFragmentType fragmentType = CentipedeFragmentType.Body;
        
        public float MaxHealth { get; set; }
        private bool First => frontFragment is null;
        private bool Last => backFragment is null;
        
        
        
        private void Awake()
        {
            hitbox = GetComponentInChildren<CentipedeHitbox>();
            hitbox.Fragment = this;
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            painter = GetComponent<BodyPainter>();
        }

        private void Start()
        {
            currentHealth = MaxHealth;
            healthbar = HealthbarPool.Instance.Create(this);
        }

        private void FixedUpdate()
        {
            if(frontFragment is null) return;
            Move(frontFragment.transform.position, 100, 30);
        }

        private void SetHealth(float hp)
        {
            currentHealth = hp;
            healthbar.SetValue(Mathf.Clamp01(currentHealth / MaxHealth));
        }

        public float Damage(
            float damage, 
            Vector3 position = default, 
            float knockback = 0, 
            float stunDuration = 0, 
            Color damageColor = default,
            bool ignoreArmor = false,
            AttackEffect effect = null)
        {
            // Multiple collisions may happen in a single frame so this check is required to
            // make sure that the fragment dies only once
            if (dead || !hitbox.Enabled) return 0;
            damage = ignoreArmor ? damage : PhysicsUtility.CalculateDamage(damage, CentipedeDefinitions.Armor * (int) fragmentType * 0.5f);
            StatRecorder.damageDealt += damage;
            SetHealth(currentHealth - damage);
            painter.Paint(new Gradient().FastGradient(Color.white, spriteRenderer.color), GlobalDefinitions.EnemyImmunityDuration);
            
            if (currentHealth <= 0)
            {
                Bossbar.Instance.Damage(damage + currentHealth);
                DieFromAttack();
            }
            else
            {
                hitbox.Hit();
                Bossbar.Instance.Damage(damage);
            }
            
            effect?.Impact(this, damage);
            
            return damage;
        }

        private void DieFromDestruction()
        {
            Bossbar.Instance.Damage(currentHealth);
            SetHealth(0);
            DieFromAttack();
        }
        
        private void DieFromAttack()
        {
            dead = true;
            hitbox.Die();
            
            if (frontFragment is not null)
            {
                frontFragment.backFragment = null;
                if (frontFragment.First)
                    frontFragment.DieFromDestruction();
                else
                    frontFragment.gameObject.AddComponent(typeof(CentipedeTail));
            }

            if (backFragment is not null)
            {
                backFragment.frontFragment = null;
                if (backFragment.Last)
                    backFragment.DieFromDestruction();
                else
                    backFragment.gameObject.AddComponent(typeof(CentipedeHead));
            }
            
            CentipedeBoss.Instance.OnFragmentDeath();
            StartCoroutine(DespawnRoutine());
        }

        private IEnumerator DespawnRoutine()
        {
            if(gameObject.TryGetComponent(out CentipedeHead head)) Destroy(head);
            if(sprayCooldownRoutine is not null) StopCoroutine(sprayCooldownRoutine);
            sprayParticles.gameObject.SetActive(false);
            locator.gameObject.SetActive(false);
            attackGO.SetActive(false);
            spriteRenderer.sortingLayerName = "Ground";
            spriteRenderer.sortingOrder = 100;
            painter.Paint(new Gradient().FastGradient(spriteRenderer.color, GlobalDefinitions.DeadColor), 1f);
            enabled = false;
            rb.linearDamping = 1f;

            if (fragmentType is CentipedeFragmentType.Body)
                rb.linearVelocity = transform.up * 2;
            else
            {
                rb.AddForce(Random.insideUnitCircle.normalized * 2f, ForceMode2D.Impulse);
                rb.angularVelocity = 720f;
                rb.angularDamping = 2f;
            }
            yield return new WaitForSeconds(6f);
            painter.FadeOut(2f);
            yield return new WaitForSeconds(2.1f);
            Destroy(gameObject);
        }

        public void CreateFragment(int posFromTail, int length) 
            => StartCoroutine(CreateFragmentRoutine(posFromTail, length));

        private IEnumerator CreateFragmentRoutine(int posFromTail, int length)
        {
            gameObject.name = $"{gameObject.name}_{posFromTail}";
            yield return new WaitForSeconds(0.1f);
            CentipedeFragment fragment = Instantiate(CentipedeDefinitions.FragmentPrefab).GetComponent<CentipedeFragment>();
            backFragment = fragment;
            fragment.frontFragment = this;
            fragment.transform.position = transform.position;
            fragment.transform.SetParent(CentipedeBoss.Instance.transform);
            fragment.MaxHealth = MaxHealth;
            fragment.UpdateColor((float) posFromTail / length);
            Bossbar.Instance.AddMaxHealth(CentipedeDefinitions.FragmentHealth);
            if (posFromTail == 1) fragment.gameObject.AddComponent(typeof(CentipedeTail));
            else if(posFromTail != 0) fragment.CreateFragment(posFromTail - 1, length);
        }

        public void Move(Vector2 target, float maxSpeed, float rotationSpeed)
        {
            Vector2 pos = transform.position;
            float distance = (target - pos).sqrMagnitude;
            rb.RotateTowardsPosition(target, rotationSpeed);

            float speed = CentipedeDefinitions.FragmentSpeed * distance;
            if (speed > maxSpeed) speed = maxSpeed;
            rb.linearVelocity = distance < CentipedeDefinitions.FollowRadius ?
                Vector2.zero : transform.up * speed;
        }

        public void PlayAnimation(int hash) => animator.Play(hash);

        public void SetHead()
        {
            gameObject.name = "CentipedeHead";
            attackGO.SetActive(true);
            fragmentType = CentipedeFragmentType.Head;
        }

        public void SetTail()
        {
            gameObject.name = "CentipedeTail";
            sprayParticles.gameObject.SetActive(true);
            locator.gameObject.SetActive(true);
            particleCollisionProvider.OnCollision += OnBulletCollision;
            locator.OnTargetLocated += OnPlayerLocated;
            fragmentType = CentipedeFragmentType.Tail;
        }

        public void UpdateColor(float value) => spriteRenderer.color = CentipedeDefinitions.GetFragmentColor(value);

        private IEnumerator SprayCooldownRoutine()
        {
            locator.OnTargetLocated -= OnPlayerLocated;
            yield return new WaitForSeconds(5f);
            locator.OnTargetLocated += OnPlayerLocated;
        }
        
        private void OnBulletCollision(IDamageable damageable)
        {
            if (damageable is PlayerManager manager)
            {
                manager.Damage(
                    CentipedeDefinitions.PoisonDamage, 
                    rb.position,
                    CentipedeDefinitions.Knockback * 0.25f,
                    0, Color.white, true);
            }
        }

        private void OnPlayerLocated(ILocatorTarget target)
        {
            if(target is PlayerMovement)
            {
                sprayCooldownRoutine = StartCoroutine(SprayCooldownRoutine());
                sprayParticles.Play();
            }
        }

        private void OnDestroy()
        {
            OnProviderDestroy?.Invoke();
            particleCollisionProvider.OnCollision -= OnBulletCollision;
            locator.OnTargetLocated -= OnPlayerLocated;
        }
        
        

        // IDamageable
        public event IDestructionEventProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => -0.5f;
        public float HealthbarWidth => 80;
    }
}