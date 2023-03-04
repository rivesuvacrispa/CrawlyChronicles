using Definitions;
using UnityEngine;
using Util;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Enemy")]
    public class Enemy : ScriptableObject
    {
        [Header("Utility fields")] 
        [SerializeField] private bool hasAttackAnimation;
        [SerializeField] private string animatorName;
        [SerializeField] private float healthbarOffsetY;
        [SerializeField] private float healthbarWidth;
        [SerializeField] private Color bodyColor;

        [Header("Dependent stats")]
        [SerializeField] private float maxHealth;
        [SerializeField] private float attackPower;
        [SerializeField] private float damage;
        [SerializeField] private float armor;

        [Header("Undepended stats")] 
        [SerializeField] private float attackDelay = 0.75f;
        [SerializeField] private float attackDistance = 1;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float locatorRadius;
        [SerializeField] private int wanderingRadius;
        [SerializeField, Range(0, 2f)] private float playerMass;
        
        [Header("Audio")]
        [SerializeField] private AudioClip hitAudio;
        [SerializeField] private AudioClip attackAudio;
        [SerializeField] private AudioClip crawlAudio;
        [SerializeField] private AudioClip deathAudio;
        [SerializeField, ShowOnly] private float crawlPitch;

        private Gradient immunityGradient;
        private Gradient poisonGradient;
        private Gradient lifestealGradient;
        private bool effectGradientsInitialized;


        public float AttackDistance => attackDistance;
        public float AttackDelay => attackDelay;
        public Color BodyColor => bodyColor;
        public float HealthbarOffsetY => healthbarOffsetY;
        public float HealthbarWidth => healthbarWidth;
        public int WanderingRadius => wanderingRadius;
        public float MovementSpeed => movementSpeed;
        public float LocatorRadius => locatorRadius;

        public float Mass => GlobalDefinitions.PlayerMass * playerMass;
        public AudioClip HitAudio => hitAudio;
        public AudioClip AttackAudio => attackAudio;
        public AudioClip CrawlAudio => crawlAudio;
        public AudioClip DeathAudio => deathAudio;
        public float CrawlPitch => crawlPitch;
        public bool HasAttackAnimation => hasAttackAnimation;


        public float MaxHealth => currentMaxHealth;
        public float Damage => currentDamage;
        public float AttackPower => currentAttackPower;
        public float Armor => currentArmor;


        private float currentMaxHealth;
        private float currentAttackPower;
        private float currentDamage;
        private float currentArmor;

        public int WalkAnimHash { get; private set; }
        public int IdleAnimHash { get; private set; }
        public int DeadAnimHash { get; private set; }
        public int AttackAnimHash { get; private set; }

        
        
        public void OnDifficultyChanged(Difficulty difficulty)
        {
            float multiplier = difficulty.EnemyStatsMultiplier;
            currentMaxHealth = maxHealth * multiplier;
            currentAttackPower = attackPower * multiplier;
            currentDamage = damage * multiplier;
            currentArmor = armor * multiplier;
        }
        
        private void Awake() => Init();

        private void Init()
        {
            WalkAnimHash = Animator.StringToHash(animatorName + "Walk");
            IdleAnimHash = Animator.StringToHash(animatorName + "Idle");
            DeadAnimHash = Animator.StringToHash(animatorName + "Dead");
            AttackAnimHash = Animator.StringToHash(animatorName + "Attack");
            crawlPitch = 1 + Mathf.Lerp( -0.5f, 0.5f, 1 - playerMass / 2f);
        }
        
        private void OnValidate() => Init();

    }
}