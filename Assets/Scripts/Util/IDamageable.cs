using UnityEngine;

namespace Util
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
            bool ignoreArmor = false);
    }
}