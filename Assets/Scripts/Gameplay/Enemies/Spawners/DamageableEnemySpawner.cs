using GameCycle;
using Gameplay.Mutations.AttackEffects;
using UI;
using UI.Elements;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Enemies.Spawners
{
    [RequireComponent(typeof(EnemySpawnerHitbox))]
    public class DamageableEnemySpawner : EnemySpawner, IDamageableEnemy
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float armor;


        private EnemySpawnerHitbox hitbox;
        private Healthbar healthbar;

        protected override void Start()
        {
            base.Start();
            healthbar = HealthbarPool.Instance.Create(this);
            CurrentHealth = maxHealth;
            hitbox = GetComponent<EnemySpawnerHitbox>();
        }
        
        public float HealthbarOffsetY => -0.5f;
        public float HealthbarWidth => 100;

        public event IDamageable.DeathEvent OnDeath;
        public bool Immune => !hitbox.Enabled;
        public float Armor => armor;
        public float CurrentHealth { get; set; }
        
        private void UpdateHealthbar() => healthbar.SetValue(Mathf.Clamp01(CurrentHealth / maxHealth));
        

        public void OnBeforeHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false, AttackEffect effect = null)
        {
            UpdateHealthbar();
        }

        public void OnLethalHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false, AttackEffect effect = null)
        {
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }

        public void OnHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false, AttackEffect effect = null)
        {
            hitbox.Hit();
        }
    }
}