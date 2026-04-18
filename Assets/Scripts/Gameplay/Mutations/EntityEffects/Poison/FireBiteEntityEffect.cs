using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.EntityEffects.Poison
{
    public class FireBiteEntityEffect : EntityEffect, IDamageSource
    {
        private float damageBuffer = 0;
        
        protected override void OnApplied()
        {
            FireBiteEffectData data = (FireBiteEffectData) Data;
            if (damageBuffer == 0)
                damageBuffer += data.TotalDamage;
        }

        protected override void OnRefreshed(EntityEffectData data)
        {
            damageBuffer += ((FireBiteEffectData)data).TotalDamage;
        }

        protected override void Tick()
        {
            if (Target is not IDamageableEnemy enemy || 
                damageBuffer == 0)
            {
                Cancel();
                return;
            }
            float damage = damageBuffer / (Data.DurationInSeconds * TICKS_PER_SECOND);
            damage = Mathf.Clamp(damage, 0.01f, float.MaxValue);
            enemy.Damage(
                new DamageSource(this, Time.frameCount),
                damage,
                PlayerPhysicsBody.Position,
                damageColor: Color.red,
                piercing: true
            );
        }

        protected override void OnRemoved()
        {
            damageBuffer = 0;
        }
    }
}