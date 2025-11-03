using Definitions;
using Gameplay.Enemies;
using Hitboxes;
using Util.Interfaces;

namespace Gameplay.Mutations.EntityEffects.Poison
{
    public class PoisonEntityEffect : EntityEffect, IDamageSource
    {
        protected override void OnApplied()
        {
            PoisonEffectData data = (PoisonEffectData) Data;
            if (Target is Enemy enemy)
                enemy.SetMovementSpeed(1 - data.Slow);
        }

        protected override void Tick()
        {
            if(Target is not IDamageableEnemy enemy || TickCounter % 4 != 0) return;
            
            PoisonEffectData data = (PoisonEffectData) Data;
            enemy.Damage(new DamageInstance(
                new DamageSource(this, TickCounter), 
                data.Damage, 
                Player.PlayerPhysicsBody.Position, 
                0.1f,
                0, 
                GlobalDefinitions.PoisonColor,
                true)
                );
        }

        protected override void OnRemoved()
        {
            if(Target is Enemy enemy)
                enemy.SetMovementSpeed(1f);
        }
    }
}