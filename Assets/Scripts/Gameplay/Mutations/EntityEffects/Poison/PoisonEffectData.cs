namespace Gameplay.Abilities.EntityEffects
{
    public class PoisonEffectData : EntityEffectData
    {
        public float Slow { get; }
        public float Damage { get; }
        
        public PoisonEffectData(int duration, float slow, float damage) : base(duration)
        {
            Slow = slow;
            Damage = damage;
        }
    }
}