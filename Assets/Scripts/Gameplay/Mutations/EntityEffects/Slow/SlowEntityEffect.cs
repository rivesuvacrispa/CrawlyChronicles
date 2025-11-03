using Gameplay.Enemies;

namespace Gameplay.Mutations.EntityEffects.Slow
{
    public class SlowEntityEffect : EntityEffect
    {
        protected override void OnApplied()
        {
            SlowEffectData data = (SlowEffectData) Data;
            if (Target is Enemy enemy)
            {
                enemy.SetMovementSpeed(1 - data.MovementSlow);
                enemy.SetRotationSpeed(1 - data.RotationSlow);
            }
        }

        protected override void Tick()
        {

        }

        protected override void OnRemoved()
        {
            if (Target is Enemy enemy)
            {
                enemy.SetMovementSpeed(1);
                enemy.SetRotationSpeed(1);
            }
        }
    }
}