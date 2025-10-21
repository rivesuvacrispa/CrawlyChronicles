using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Enemies.Spawners
{
    [RequireComponent(typeof(EnemySpawnerHitbox))]
    public class DamageableEnemySpawner : EnemySpawner, IDamageableEnemy
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float armor;


        private EnemySpawnerHitbox hitbox;

        protected override void Start()
        {
            base.Start();
            CurrentHealth = maxHealth;
            hitbox = GetComponent<EnemySpawnerHitbox>();
        }
        
        public float HealthbarOffsetY => -0.5f;
        public float HealthbarWidth => 100;

        public event IDamageable.DeathEvent OnDeath;
        public event IDamageable.DamageEvent OnDamageTaken;
        public bool Immune => !hitbox.Enabled;
        public float Armor => armor;
        public float CurrentHealth { get; set; }
        public float MaxHealth => maxHealth;

        

        public void OnBeforeHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            OnDamageTaken?.Invoke(this, damage);
        }

        public void OnLethalHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }

        public void OnHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            hitbox.Hit();
        }
    }
}