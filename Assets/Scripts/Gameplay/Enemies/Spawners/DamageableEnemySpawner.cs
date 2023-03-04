using UI;
using UnityEngine;
using Util;

namespace Gameplay.Enemies.Spawners
{
    [RequireComponent(typeof(EnemySpawnerHitbox))]
    public class DamageableEnemySpawner : EnemySpawner, IDamageableEnemy
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float armor;


        private EnemySpawnerHitbox hitbox;
        private Healthbar healthbar;
        private float currentHealth;

        protected override void Start()
        {
            base.Start();
            healthbar = HealthbarPool.Instance.Create(this);
            currentHealth = maxHealth;
            hitbox = GetComponent<EnemySpawnerHitbox>();
        }
        
        public float HealthbarOffsetY => -0.5f;
        public float HealthbarWidth => 100;

        public float Damage(float damage, 
            Vector3 position = default, 
            float knockback = 0, 
            float stunDuration = 0,
            Color damageColor = default,
            bool ignoreArmor = false)
        {
            if (!hitbox.Enabled) return 0;
            damage = ignoreArmor ? damage : PhysicsUtility.CalculateDamage(damage, armor);
            currentHealth -= damage;
            healthbar.SetValue(Mathf.Clamp01(currentHealth / maxHealth));
            if (currentHealth <= 0)
                Destroy(gameObject);
            else 
                hitbox.Hit();
            return damage;
        }
    }
}