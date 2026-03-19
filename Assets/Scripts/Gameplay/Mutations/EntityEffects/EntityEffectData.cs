namespace Gameplay.Mutations.EntityEffects
{
    public class EntityEffectData
    {
        public int DurationInSeconds { get; }

        public EntityEffectData(int durationInSecondsInSeconds)
        {
            DurationInSeconds = durationInSecondsInSeconds;
        }
    }
}