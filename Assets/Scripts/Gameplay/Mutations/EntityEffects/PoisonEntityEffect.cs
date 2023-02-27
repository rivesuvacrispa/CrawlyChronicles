using Definitions;

namespace Gameplay.Abilities.EntityEffects
{
    public class PoisonEntityEffect : EntityEffect
    {
        protected override void OnApplied()
        {
            PoisonEffectData data = (PoisonEffectData) Data;
            Enemy.SetMovementSpeed(1 - data.Slow);
        }

        protected override void Tick()
        {
            PoisonEffectData data = (PoisonEffectData) Data;
            Enemy.Damage(data.Damage, 0.1f, 0, GlobalDefinitions.PoisonColor, true);
        }

        protected override void OnRemoved() => Enemy.SetMovementSpeed(1f);
    }
}