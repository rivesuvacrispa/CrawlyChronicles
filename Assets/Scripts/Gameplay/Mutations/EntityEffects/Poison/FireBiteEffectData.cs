namespace Gameplay.Mutations.EntityEffects.Poison
{
    public class FireBiteEffectData : EntityEffectData
    {
        public float TotalDamage { get; }
        
        public FireBiteEffectData(int durationInSeconds, float totalDamage) 
            : base(durationInSeconds)
        {
            TotalDamage = totalDamage;
        }
    }
}