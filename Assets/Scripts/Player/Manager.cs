using Gameplay.Genetics;
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
        
        [field: SerializeReference] private PlayerStats playerStats = new();
        public bool IsHoldingEgg { get; private set; }
        public TrioGene HoldingEgg { get; private set; }
        public bool AllowInteract => !attackController.IsAttacking;
        public static PlayerStats PlayerStats => Instance.playerStats;
        
        private int health;
        private int foodAmount;
        
        
        
        private void Awake() => Instance = this;


        private void Start()
        {
            healthbar.SetTarget(this);
            health = playerStats.MaxHealth;
        }

        public void PickEgg(TrioGene gene)
        {
            HoldingEgg = gene;
            IsHoldingEgg = true;
            eggSpriteRenderer.enabled = true;
            attackController.enabled = false;
        }

        public void RemoveEgg()
        {
            IsHoldingEgg = false;
            eggSpriteRenderer.enabled = false;
            attackController.enabled = true;
        }

        public void AddHealth(int amount)
        {
            health = Mathf.Clamp(health + amount, health, playerStats.MaxHealth);
            UpdateHealthbar();
        }
        
        public void Damage(int damage)
        {
            health -= damage;
            UpdateHealthbar();
            if (health <= 0)
            {
                health = 0;
                Die();
            }
        }

        private void UpdateHealthbar()
        {
            healthbar.SetValue(health, (float) health / playerStats.MaxHealth);
        }
        
        private void Die()
        {
            AddHealth(100);
        }

        
        
        // IDamageable
        public Transform Transform => transform;
        public float HealthbarOffsetY => healthbarOffsetY;
        public float HealthbarWidth => healthbarWidth;
    }
}