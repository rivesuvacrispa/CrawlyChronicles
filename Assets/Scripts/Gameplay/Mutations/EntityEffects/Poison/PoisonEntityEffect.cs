using Definitions;
using Gameplay.Enemies;
using Util.Interfaces;

namespace Gameplay.Abilities.EntityEffects
{
    public class PoisonEntityEffect : EntityEffect
    {
        protected override void OnApplied()
        {
            PoisonEffectData data = (PoisonEffectData) Data;
            if(Target is Enemy enemy)
                enemy.SetMovementSpeed(1 - data.Slow);
        }

        protected override void Tick()
        {
            if(Target is not IDamageableEnemy enemy) return;
            
            PoisonEffectData data = (PoisonEffectData) Data;
            enemy.Damage(
                data.Damage, 
                Player.PlayerMovement.Position, 
                0.1f,
                0, 
                GlobalDefinitions.PoisonColor,
                true);
        }

        protected override void OnRemoved()
        {
            if(Target is Enemy enemy)
                enemy.SetMovementSpeed(1f);
        }
    }
}