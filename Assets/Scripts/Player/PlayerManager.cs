using Definitions;
using GameCycle;
using Gameplay;
using Gameplay.Abilities.EntityEffects;
using Gameplay.Interaction;
using Mutations.AttackEffects;
using Scripts.SoundEffects;
using Timeline;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Util.Interfaces;

namespace Player
{
    public class PlayerManager : MonoBehaviour, IDamageable
    {
        public static PlayerManager Instance { get; private set; }
        
#if UNITY_EDITOR
        [field:SerializeField] public bool GodMode { get; private set; }
#endif
        
        [SerializeField] private PlayerHitbox hitbox;
        [SerializeField] private ParticleSystem healingParticles;
        [SerializeField] private Animator spriteAnimator;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private Text healthText;
        [SerializeField] private Text statsText;
        [SerializeField] private Healthbar healthbar;
        [SerializeField] private float healthbarOffsetY;
        [SerializeField] private float healthbarWidth;
        [SerializeField] private SpriteRenderer eggSpriteRenderer;
        [SerializeField] private AttackController attackController;
        [SerializeField] private PlayerStats baseStats;
        [SerializeField] private PlayerStats currentStats;

        public delegate void PlayerManagerEvent();
        public static event PlayerManagerEvent OnPlayerKilled;
        
        private readonly int deadHash = Animator.StringToHash("PlayerSpriteDead");
        
        public bool IsHoldingEgg { get; private set; }
        public Egg HoldingEgg { get; private set; }
        public bool AllowInteract => !attackController.IsAttacking;
        public static PlayerStats PlayerStats => Instance.currentStats;
        
        private float health;
        
        
        
        public PlayerManager() => Instance = this;

        private void Awake()
        {
            currentStats = baseStats;
            MainMenu.OnResetRequested += OnResetRequested;
        }
        
        private void Start()
        {
            healthbar.SetTarget(this);
            health = currentStats.MaxHealth;
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
            currentStats.AddStats(baseStats, stats);
            statsText.text = currentStats.Print();
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
            if(health < 0 || health >= currentStats.MaxHealth) return;
            healingParticles.Play();
            health = Mathf.Clamp(health + amount, health, currentStats.MaxHealth);
            UpdateHealthbar();
        }
        
        public float Damage(
            float damage, 
            Vector3 position,
            float knockback, 
            float stunDuration = 0, 
            Color damageColor = default,
            bool ignoreArmor = false,
            AttackEffect effect = null)
        {
#if UNITY_EDITOR
            if(GodMode) return 0;
#endif
            
            if (hitbox.Immune) return 0;
            PlayerAudioController.Instance.PlayHit();
            movement.Knockback(position, knockback);
            damage = ignoreArmor ? damage : PhysicsUtility.CalculateDamage(damage, currentStats.Armor);
            health -= damage;
            UpdateHealthbar();
            if (health <= float.Epsilon) Die(true);
            else hitbox.Hit();
            return damage;
        }
        
        public void Knockback(Vector2 pos, float knockback) => movement.Knockback(pos, knockback);

        private void UpdateHealthbar()
        {
            float value = health / currentStats.MaxHealth;
            healthbar.SetValue(value);
            healthText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
        
        public void Die(bool invokeDeathEvent)
        {
            if(invokeDeathEvent) OnPlayerKilled?.Invoke();
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
            StatRecorder.respawns++;
            health = currentStats.MaxHealth;
            UpdateHealthbar();
            TimeManager.OnDayStart += OnDayStart;
        }


        private void OnDestroy()
        {
            OnProviderDestroy?.Invoke();
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
            currentStats = baseStats;
            health = currentStats.MaxHealth;
            statsText.text = currentStats.Print();
            healthText.text = Mathf.CeilToInt(health).ToString();
            UpdateHealthbar();
        }


        // IDamageable
        public event IDamageable.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => healthbarOffsetY;
        public float HealthbarWidth => healthbarWidth;
    }
}