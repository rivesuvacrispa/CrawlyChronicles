using Gameplay.Player;

namespace Gameplay.Mutations.EntityEffects.Stat
{
    public abstract class StatPlayerEffect : EntityEffect
    {
        private PlayerStats latestAddedStats = PlayerStats.Zero;
        

        protected override void OnApplied()
        {
            if (!Target.Equals(PlayerManager.Instance))
            {
                Cancel();
                return;
            }
            
            StatEffectData data = (StatEffectData) Data;
            PlayerStats stats = data.Stats;
            PlayerManager.Instance.AddStats(stats);
            latestAddedStats = stats;
        }

        protected override void Tick()
        {
            
        }

        protected override void OnRemoved()
        {
            PlayerManager.Instance.AddStats(latestAddedStats.Negated());
            latestAddedStats = PlayerStats.Zero;
        }
    }
}