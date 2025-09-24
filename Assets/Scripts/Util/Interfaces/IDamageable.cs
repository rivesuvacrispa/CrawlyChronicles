using Gameplay.Mutations.AttackEffects;
using UnityEngine;

namespace Util.Interfaces
{
    public interface IDamageable : ITransformProvider
    {
        public float HealthbarOffsetY { get; }
        public float HealthbarWidth { get; }

        public float Damage(
            float damage,
            Vector3 position, 
            float knockback, 
            float stunDuration, 
            Color damageColor,
            bool ignoreArmor = false,
            AttackEffect effect = null);

        public delegate void DamageEvent(float damage);
        public static event DamageEvent OnDamageTaken;
        public void InvokeDamageTakenEvent(float damage) => OnDamageTaken?.Invoke(damage);
    }
}