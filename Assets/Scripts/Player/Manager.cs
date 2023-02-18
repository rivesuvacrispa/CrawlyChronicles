using Definitions;
using Gameplay;
using UI;
using UnityEngine;

namespace Player
{
    public class Manager : MonoBehaviour, IDamageable
    {
        public static Manager Instance { get; private set; }
        
        [SerializeField] private Healthbar healthbar;
        [SerializeField] private float healthbarOffsetY;
        [SerializeField] private float healthbarWidth;
        [SerializeField] private SpriteRenderer eggSpriteRenderer;
        [SerializeField] private AttackController attackController;
        [SerializeField] private PlayerStats baseStats = new();
        [SerializeField] private PlayerStats currentStats;
        
        [field:SerializeField] public bool GodMode { get; private set; }
        public bool IsHoldingEgg => HoldingEgg is not null;
        public Egg HoldingEgg { get; private set; }
        public bool AllowInteract => !attackController.IsAttacking;
        public static PlayerStats PlayerStats => Instance.currentStats;
        
        private float health;
        private int foodAmount;
        
        
        
        private void Awake()
        {
            Instance = this;
            currentStats = baseStats;
        }
        
        private void Start()
        {
            healthbar.SetTarget(this);
            health = currentStats.MaxHealth;
        }

        private void Update()
        {
            if(IsHoldingEgg && Input.GetKeyDown(KeyCode.Escape)) DropEgg();
        }

        public void AddStats(PlayerStats stats)
        {
            currentStats.AddStats(baseStats, stats);
        }
        
        public void PickEgg(Egg gene)
        {
            HoldingEgg = gene;
            eggSpriteRenderer.enabled = true;
            attackController.enabled = false;
        }

        public void RemoveEgg()
        {
            eggSpriteRenderer.enabled = false;
            attackController.enabled = true;
        }
        
        private void DropEgg()
        {
            RemoveEgg();
            var egg = GlobalDefinitions.CreateEggDrop(HoldingEgg).transform;
            egg.position = (Vector3) Movement.Position + transform.up * 0.35f;
            egg.rotation = Quaternion.Euler(0, 0, Movement.Rotation);
        }

        public void AddHealth(float amount)
        {
            health = Mathf.Clamp(health + amount, health, currentStats.MaxHealth);
            UpdateHealthbar();
        }
        
        public void Damage(float damage)
        {
            health -= damage;
            UpdateHealthbar();
            if (health <= float.Epsilon)
            {
                health = 0;
                Die();
            }
        }

        private void UpdateHealthbar()
        {
            healthbar.SetValue(health / currentStats.MaxHealth);
        }
        
        private void Die()
        {
            BreedingManager.Instance.Abort();
            if(IsHoldingEgg) DropEgg();
            if (BreedingManager.Instance.CanRespawn)
            {
                //Respawn
                AddHealth(100);
            }
            else
            {
                Debug.Log("LOL U DIED LOL LOL U DIED");
            }
        }


        private void OnDestroy() => OnDamageableDestroy?.Invoke();
        
        

        // IDamageable
        public event IDamageable.DamageableEvent OnDamageableDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => healthbarOffsetY;
        public float HealthbarWidth => healthbarWidth;
    }
}