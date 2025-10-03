using Gameplay.Mutations.AttackEffects;
using UnityEngine;

namespace Util.Interfaces
{
    public interface IDamageable : ITransformProvider
    {
        public float HealthbarOffsetY { get; }
        public float HealthbarWidth { get; }
        
        public delegate void GlobalDamageEvent(IDamageable damageable, float damage);
        public static event GlobalDamageEvent OnDamageTakenGlobal;

        public delegate void DeathEvent(IDamageable damageable);
        public static event DeathEvent OnLethalBlow;

        public bool Immune { get; }
        public float Armor { get; }
        public float CurrentHealth { get; set; }

        public float Damage(
            float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false,
            AttackEffect effect = null)
        {
            if (Immune) return 0;

            if (damage is float.NaN or 0f)
                damage = float.Epsilon;

            if (TryBlockDamage(damage, position, knockback, stunDuration, damageColor, piercing, effect))
                return 0;
            
            damage = piercing ? damage : PhysicsUtility.CalculateDamage(damage, Armor);

            CurrentHealth -= damage;
            Debug.Log($"{((Component)this).gameObject.name} damaged for {damage}, piercing: {piercing}");
            OnDamageTakenGlobal?.Invoke(this, damage);
            OnBeforeHit(damage, position, knockback, stunDuration, damageColor, piercing, effect);
            
            if (CurrentHealth <= float.Epsilon)
            {
                OnLethalBlow?.Invoke(this);
                OnLethalHit(damage, position, knockback, stunDuration, damageColor, piercing, effect);
            }
            else
                OnHit(damage, position, knockback, stunDuration, damageColor, piercing, effect);
            
            if (effect is not null && this is IImpactable impactable)
                effect.Impact(impactable, damage);

            return damage;
        }

        public void OnBeforeHit(
            float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false,
            AttackEffect effect = null)
        {
            
        }

        public bool TryBlockDamage(
            float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false,
            AttackEffect effect = null) => false;

        public void OnLethalHit(
            float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false,
            AttackEffect effect = null);

        public void OnHit(
            float damage,
            Vector3 position,
            float knockback,
            float stunDuration,
            Color damageColor,
            bool piercing = false,
            AttackEffect effect = null);
    }
}