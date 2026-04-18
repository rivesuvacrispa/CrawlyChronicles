namespace Gameplay.Mutations.EntityEffects
{
    public class EntityEffectData
    {
        public int DurationInSeconds { get; }

        public EntityEffectData(int durationInSeconds)
        {
            DurationInSeconds = durationInSeconds;
        }
    }
}