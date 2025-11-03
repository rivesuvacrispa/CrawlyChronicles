using Gameplay.Player;

namespace Gameplay.Mutations.EntityEffects.Stat
{
    public class StatEffectData : EntityEffectData
    {
        public PlayerStats Stats { get; }
        
        public StatEffectData(int duration, PlayerStats stats) : base(duration)
        {
            Stats = stats;
        }
    }
}