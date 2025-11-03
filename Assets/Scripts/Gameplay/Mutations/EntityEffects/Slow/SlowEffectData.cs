namespace Gameplay.Mutations.EntityEffects.Slow
{
    public class SlowEffectData : EntityEffectData
    {
        public float MovementSlow { get; }
        public float RotationSlow { get; }

        public SlowEffectData(int duration, float movementSlow, float rotationSlow) : base(duration)
        {
            MovementSlow = movementSlow;
            RotationSlow = rotationSlow;
        }
    }
}