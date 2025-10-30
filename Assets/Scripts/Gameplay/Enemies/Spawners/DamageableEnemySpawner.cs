using Definitions;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Enemies.Spawners
{
    public class DamageableEnemySpawner : EnemySpawner, IDamageableEnemy
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float armor;
        [SerializeField] private DamageableEnemyHitbox hitbox;
        [SerializeField] private BodyPainter bodyPainter;



        protected override void Start()
        {
            base.Start();
            CurrentHealth = maxHealth;
        }
        
        public float HealthbarOffsetY => -0.5f;
        public float HealthbarWidth => 100;

        public event IDamageable.DeathEvent OnDeath;
        public event IDamageable.DamageEvent OnDamageTaken;
        public bool ImmuneToSource(DamageSource source) => hitbox.ImmuneToSource(source);
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
            hitbox.Hit(instance);
            bodyPainter.FadeOut(GlobalDefinitions.EnemyImmunityDuration);
            // TODO: Audio effect
        }
    }
}