using System.Collections.Generic;
using Gameplay.Effects.DamageText;
using Gameplay.Mutations.AttackEffects;
using Pooling;
using UnityEngine;

namespace Util.Interfaces
{
    public interface IDamageable : ITransformProvider
    {
        public float HealthbarOffsetY { get; }
        public float HealthbarWidth { get; }
        
        public delegate void GlobalDamageEvent(IDamageable damageable, float damage);
        public static event GlobalDamageEvent OnDamageTakenGlobal;

        public delegate void GlobalDeathEvent(IDamageable damageable);
        public static event GlobalDeathEvent OnLethalBlowGlobal;
        
        public delegate void DeathEvent(IDamageable damageable);
        public event DeathEvent OnDeath;

        public delegate void DamageEvent(IDamageable damageable, float damage);
        public event DamageEvent OnDamageTaken;

        public bool Immune { get; }
        public float Armor { get; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; }

        public float Damage(float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false,
            List<AttackEffect> effects = null)
        {
            Struck();
            if (Immune) return 0;

            if (damage is float.NaN or 0f)
                damage = float.Epsilon;
            
            if (damageColor == default) damageColor = Color.white;

            if (TryBlockDamage(damage, position, knockback, stunDuration, damageColor, piercing))
                return 0;
            
            damage = piercing ? damage : PhysicsUtility.CalculateDamage(damage, Armor);

            CurrentHealth -= damage;
            Debug.Log($"{((Component)this).gameObject.name} damaged for {damage}, piercing: {piercing}");
            OnDamageTakenGlobal?.Invoke(this, damage);
            OnBeforeHit(damage, position, knockback, stunDuration, damageColor, piercing);

            PoolManager.GetEffect<DamageText>(new DamageTextArguments(Transform.position, damage));
            if (CurrentHealth <= float.Epsilon)
            {
                OnLethalBlowGlobal?.Invoke(this);
                OnLethalHit(damage, position, knockback, stunDuration, damageColor, piercing);
            }
            else
            {
                OnHit(damage, position, knockback, stunDuration, damageColor, piercing);
            }
            
            if (effects is not null && this is IImpactable impactable)
                foreach (AttackEffect effect in effects)
                    effect.Impact(impactable, damage);

            return damage;
        }

        public void OnBeforeHit(float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false);
        
        public void Struck() { }

        public bool TryBlockDamage(float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false) => false;

        public void OnLethalHit(float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false);

        public void OnHit(float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false);
    }
}