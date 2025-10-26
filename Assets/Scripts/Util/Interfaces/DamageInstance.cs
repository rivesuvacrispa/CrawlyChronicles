using System.Collections.Generic;
using Gameplay.Mutations.AttackEffects;
using UnityEngine;

namespace Util.Interfaces
{
    public struct DamageInstance
    {
        private readonly float rawDamage;
        
        public float Damage { get; private set; }
        public readonly DamageSource source;
        public readonly Vector3 position;
        public readonly float knockback;
        public readonly float stunDuration;
        public readonly Color damageColor;
        public readonly bool piercing;
        public readonly List<AttackEffect> effects;

        public DamageInstance(
            DamageSource source, 
            float rawDamage, 
            Vector3 position = default, 
            float knockback = 0f, 
            float stunDuration = 0f,
            Color damageColor = default,
            bool piercing = false, 
            List<AttackEffect> effects = null)
        {
            if (rawDamage is float.NaN or 0f)
                rawDamage = float.Epsilon;
            
            if (damageColor == default) damageColor = Color.white;

            this.source = source;
            this.rawDamage = rawDamage;
            this.position = position;
            this.knockback = knockback;
            this.stunDuration = stunDuration;
            this.damageColor = damageColor;
            this.piercing = piercing;
            this.effects = effects;
            Damage = rawDamage;
        }

        public float CalculateDamage(float armor)
        {
            Damage = piercing ? rawDamage : PhysicsUtility.CalculateDamage(rawDamage, armor);
            return Damage;
        }
    }
}