using Gameplay.Effects.DamageText;
using Gameplay.Mutations.AttackEffects;
using Pooling;
using UnityEngine;
using Util.Interfaces;

namespace Hitboxes
{
    public interface IDamageable : ITransformProvider
    {
        public float HealthbarOffsetY { get; }
        public float HealthbarWidth { get; }
        
        public delegate void GlobalDamageEvent(IDamageable damageable, DamageInstance instance);
        public static event GlobalDamageEvent OnDamageTakenGlobal;

        public delegate void GlobalDeathEvent(IDamageable damageable);
        public static event GlobalDeathEvent OnLethalBlowGlobal;
        
        public delegate void DeathEvent(IDamageable damageable);
        public event DeathEvent OnDeath;

        public delegate void DamageEvent(IDamageable damageable, DamageInstance instance);
        public event DamageEvent OnDamageTaken;

        public float Armor { get; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; }
        public IDamageableHitbox Hitbox { get; }

        public float Damage(DamageInstance instance)
        {
            if (Hitbox.ImmuneToSource(instance.source)) return 0;
            Struck();

            if (TryBlockDamage(instance))
                return 0;

            float damage = instance.CalculateDamage(Armor);
            CurrentHealth -= damage;
            Debug.Log($"{((Component)this).gameObject.name} damaged for {damage}, piercing: {instance.piercing}");
            OnDamageTakenGlobal?.Invoke(this, instance);
            OnBeforeHit(instance);

            PoolManager.GetEffect<DamageText>(new DamageTextArguments(Transform.position, damage));
            if (CurrentHealth <= float.Epsilon)
            {
                OnLethalBlowGlobal?.Invoke(this);
                OnLethalHit(instance);
                Hitbox.Die();
            }
            else
            {
                OnHit(instance);
                Hitbox.Hit(instance);
            }
            
            if (instance.effects is not null && this is IImpactable impactable)
                foreach (AttackEffect effect in instance.effects)
                    effect.Impact(impactable, damage);

            return damage;
        }

        public void OnBeforeHit(DamageInstance damageInstance);
        
        public void Struck() { }

        public bool TryBlockDamage(DamageInstance damageInstance) => false;

        public void OnLethalHit(DamageInstance damageInstance);

        public void OnHit(DamageInstance damageInstance);
    }
}