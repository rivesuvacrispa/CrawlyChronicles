using Definitions;
using UnityEngine;
using Util;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Enemy")]
    public class Enemy : ScriptableObject
    {
        [Header("Utility fields")]
        [SerializeField] private string animatorName;
        [SerializeField] private float healthbarOffsetY;
        [SerializeField] private float healthbarWidth;
        [SerializeField] private Color bodyColor;
        [Header("Stats fields")] 
        [SerializeField] private float maxHealth;
        [SerializeField] private float attackPower;
        [SerializeField] private float damage;
        [SerializeField] private float armor;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float locatorRadius;
        [SerializeField] private int wanderingRadius;
        [SerializeField, Range(0, 2f)] private float playerMass;
        [SerializeField] private AudioClip hitAudio;
        [SerializeField] private AudioClip attackAudio;
        [SerializeField] private AudioClip crawlAudio;
        [SerializeField] private AudioClip deathAudio;
        [SerializeField, ShowOnly] private float crawlPitch;

        private Gradient immunityGradient;
        private Gradient poisonGradient;
        private Gradient lifestealGradient;
        private bool effectGradientsInitialized;
        
        
        public float MaxHealth => maxHealth;
        public Color BodyColor => bodyColor;
        public float HealthbarOffsetY => healthbarOffsetY;
        public float HealthbarWidth => healthbarWidth;
        public int WanderingRadius => wanderingRadius;
        public float MovementSpeed => movementSpeed;
        public float LocatorRadius => locatorRadius;
        public float Damage => damage;
        public float AttackPower => attackPower;
        public float Armor => armor;
        public float Mass => GlobalDefinitions.PlayerMass * playerMass;
        public AudioClip HitAudio => hitAudio;
        public AudioClip AttackAudio => attackAudio;
        public AudioClip CrawlAudio => crawlAudio;
        public AudioClip DeathAudio => deathAudio;
        public float CrawlPitch => crawlPitch;

        public int WalkAnimHash { get; private set; }
        public int IdleAnimHash { get; private set; }
        public int DeadAnimHash { get; private set; }
        
        
        
        private void Awake() => Init();
        
        private void Init()
        {
            WalkAnimHash = Animator.StringToHash(animatorName + "Walk");
            IdleAnimHash = Animator.StringToHash(animatorName + "Idle");
            DeadAnimHash = Animator.StringToHash(animatorName + "Dead");
            crawlPitch = 1 + Mathf.Lerp( -0.5f, 0.5f, 1 - playerMass / 2f);
        }
        
        private void OnValidate() => Init();
    }
}