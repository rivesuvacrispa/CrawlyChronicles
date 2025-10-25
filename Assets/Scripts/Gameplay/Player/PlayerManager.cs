using Definitions;
using GameCycle;
using Gameplay.Breeding;
using Gameplay.Effects.Healthbars;
using Gameplay.Interaction;
using SoundEffects;
using Timeline;
using TMPro;
using UI.Menus;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Player
{
    public class PlayerManager : MonoBehaviour, IDamageable
    {
        public static PlayerManager Instance { get; private set; }
        
#if UNITY_EDITOR
        [Header("God Mode")]
        [field:SerializeField] public bool GodMode { get; private set; }
#endif
        
        [Header("References")]
        // TODO: rework redundant references via events
        [SerializeField] private PlayerHitbox hitbox;
        [SerializeField] private ParticleSystem healingParticles;
        [SerializeField] private Animator spriteAnimator;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerSizeManager sizeManager;
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Healthbar healthbar;
        [SerializeField] private float healthbarOffsetY;
        [SerializeField] private float healthbarWidth;
        [SerializeField] private SpriteRenderer eggSpriteRenderer;
        [SerializeField] private AttackController attackController;
        [SerializeField] private Collider2D col;
        [Header("Stats")]
        [SerializeField] private PlayerStats baseStats;
        [SerializeField] private PlayerStats currentStats;

        private readonly int deadHash = Animator.StringToHash("PlayerSpriteDead");
        public Collider2D Collider => col;
        public bool IsHoldingEgg { get; private set; }
        public Egg HoldingEgg { get; private set; }
        public bool AllowInteract => !attackController.IsAttacking;
        public PlayerSizeManager SizeManager => sizeManager;
        public static PlayerStats PlayerStats
        {
            get => Instance.currentStats;
            private set
            {
                Instance.currentStats = value;
                OnStatsChanged?.Invoke();
            }
        }

        public delegate void PlayerEvent();
        public static event PlayerEvent OnStatsChanged;
        public delegate void PlayerManagerEvent();
        public static event PlayerManagerEvent OnPlayerKilled;
        public static event PlayerManagerEvent OnPlayerRespawned;
        public delegate void PlayerHitboxEvent();
        public static event PlayerHitboxEvent OnStruck;
        
        
        
        public PlayerManager() => Instance = this;

        private void Awake()
        {
            PlayerStats = baseStats;
            MainMenu.OnResetRequested += OnResetRequested;
        }
        
        private void Start()
        {
            healthbar.SetArgs(new HealthbarArguments(this));
            CurrentHealth = currentStats.MaxHealth;
            UpdateHealthbar();
            TimeManager.OnDayStart += OnDayStart;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.R) && IsHoldingEgg) 
                DropEgg();
            else if(Input.GetKeyDown(KeyCode.K)) 
                Die(false);
            else if(Input.GetKeyDown(KeyCode.Escape) && Time.timeScale != 0) 
                mainMenu.Pause();
        }

        public void AddStats(PlayerStats stats)
        {
            currentStats.AddStats(PlayerStats.Minimal, stats);
            OnStatsChanged?.Invoke();
        }
        
        public void PickEgg(Egg egg)
        {
            IsHoldingEgg = true;
            HoldingEgg = egg;
            eggSpriteRenderer.enabled = true;
            attackController.enabled = false;
        }

        public void RemoveEgg()
        {
            IsHoldingEgg = false;
            HoldingEgg = null;
            eggSpriteRenderer.enabled = false;
            attackController.enabled = true;
        }
        
        private void DropEgg()
        {
            RemoveEgg();
            var egg = GlobalDefinitions.CreateEggDrop(HoldingEgg).transform;
            egg.position = (Vector3) PlayerMovement.Position + transform.up * 0.35f;
            egg.rotation = Quaternion.Euler(0, 0, PlayerMovement.Rotation);
        }

        public void AddHealthPercent(float percent) => AddHealth(currentStats.MaxHealth * percent);
        
        public void AddHealth(float amount)
        {
            if(CurrentHealth < 0 || CurrentHealth >= currentStats.MaxHealth) return;
            healingParticles.Play();
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, CurrentHealth, currentStats.MaxHealth);
            UpdateHealthbar();
        }
        
        public void Knockback(Vector2 pos, float knockback) => movement.Knockback(pos, knockback);

        private void UpdateHealthbar()
        {
            float value = CurrentHealth / currentStats.MaxHealth;
            healthbar.SetValue(value);
            healthText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
        
        public void Die(bool invokeDeathEvent)
        {
            if(invokeDeathEvent) OnPlayerKilled?.Invoke();
            OnDeath?.Invoke(this);
            movement.enabled = false;
            spriteAnimator.Play(deadHash);
            DeathCounter.StopCounter();
            TimeManager.OnDayStart -= OnDayStart;
            Interactor.Abort();
            BreedingManager.Instance.Abort();
            attackController.ExpireCombo();
            attackController.enabled = false;
            if(IsHoldingEgg) DropEgg();
            if (RespawnManager.CollectEggBeds() > 0)
                RespawnManager.Respawn();
            else 
                mainMenu.ShowGameOver();
        }

        public void OnRespawn()
        {
            movement.enabled = true;
            attackController.enabled = true;
            OnPlayerRespawned?.Invoke();
            CurrentHealth = currentStats.MaxHealth;
            UpdateHealthbar();
            TimeManager.OnDayStart += OnDayStart;
        }


        private void OnDestroy()
        {
            OnProviderDestroy?.Invoke(this);
            TimeManager.OnDayStart -= OnDayStart;
            MainMenu.OnResetRequested -= OnResetRequested;
        }

        private void OnDayStart(int day)
        {
            /*health = currentStats.MaxHealth;
            UpdateHealthbar();*/
        }

        private void OnResetRequested()
        {
            RemoveEgg();
            PlayerMovement.Teleport(new Vector2(15f, 15f));
            PlayerStats = baseStats;
            CurrentHealth = currentStats.MaxHealth;
            healthText.text = Mathf.CeilToInt(CurrentHealth).ToString();
            UpdateHealthbar();
        }


        // IDamageable
        public event IDamageable.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => healthbarOffsetY;
        public float HealthbarWidth => healthbarWidth;
        public event IDamageable.DeathEvent OnDeath;

#if UNITY_EDITOR
        public event IDamageable.DamageEvent OnDamageTaken;
        public bool Immune => GodMode || hitbox.Immune;
#else
        public bool Immune => hitbox.Immune;
#endif
        public float Armor => currentStats.Armor;

        public float CurrentHealth { get; set; } = 0;
        public float MaxHealth => currentStats.MaxHealth;


        public void OnLethalHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            Die(true);
        }

        public void OnHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            hitbox.Hit();
        }
        
        public void OnBeforeHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            OnDamageTaken?.Invoke(this, damage);
            PlayerAudioController.Instance.PlayHit();
            movement.Knockback(position, knockback);
            UpdateHealthbar();
        }

        public void Struck()
        {
            OnStruck?.Invoke();
        }
    }
}