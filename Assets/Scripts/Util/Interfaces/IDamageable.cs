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

        public float Armor { get; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; }

        public float Damage(DamageInstance instance)
        {
            if (ImmuneToSource(instance.source)) return 0;
            Struck();

            if (TryBlockDamage(instance))
                return 0;

            float damage = instance.CalculateDamage(Armor);
            CurrentHealth -= damage;
            Debug.Log($"{((Component)this).gameObject.name} damaged for {damage}, piercing: {instance.piercing}");
            OnDamageTakenGlobal?.Invoke(this, damage);
            OnBeforeHit(instance);

            PoolManager.GetEffect<DamageText>(new DamageTextArguments(Transform.position, damage));
            if (CurrentHealth <= float.Epsilon)
            {
                OnLethalBlowGlobal?.Invoke(this);
                OnLethalHit(instance);
            }
            else
            {
                OnHit(instance);
            }
            
            if (instance.effects is not null && this is IImpactable impactable)
                foreach (AttackEffect effect in instance.effects)
                    effect.Impact(impactable, damage);

            return damage;
        }

        public bool ImmuneToSource(DamageSource source);

        public void OnBeforeHit(DamageInstance damageInstance);
        
        public void Struck() { }

        public bool TryBlockDamage(DamageInstance damageInstance) => false;

        public void OnLethalHit(DamageInstance damageInstance);

        public void OnHit(DamageInstance damageInstance);
    }
}