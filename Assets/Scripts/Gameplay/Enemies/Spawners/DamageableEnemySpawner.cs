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
        // TODO:: immuneToSource
        public bool ImmuneToSource(DamageSource source) => !hitbox;
        public float Armor => armor;
        public float CurrentHealth { get; set; }
        public float MaxHealth => maxHealth;

        

        public void OnBeforeHit(DamageInstance instance)
        {
            OnDamageTaken?.Invoke(this, instance.Damage);
        }

        public void OnLethalHit(DamageInstance instance)
        {
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }

        public void OnHit(DamageInstance instance)
        {
            hitbox.Hit();
        }
    }
}