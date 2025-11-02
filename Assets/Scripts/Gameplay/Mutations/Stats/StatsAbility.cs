using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Mutations.Stats
{
    public class StatsAbility : BasicAbility
    {
        [SerializeField] private PlayerStats statsLvl1;
        [SerializeField] private PlayerStats statsLvl10;

        [SerializeField] private PlayerStats current = PlayerStats.Zero;

        protected bool StatsCanBeAdded =>
            !current.Equals(PlayerStats.Zero) && Application.isPlaying && isActiveAndEnabled;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if (StatsCanBeAdded) PlayerManager.Instance.AddStats(current.Negated());
            current = PlayerStats.LerpLevel(statsLvl1, statsLvl10, lvl);
            if (StatsCanBeAdded) PlayerManager.Instance.AddStats(current);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (StatsCanBeAdded)
                PlayerManager.Instance.AddStats(current);
        }
        
        protected override void OnDisable()
        {
            if (StatsCanBeAdded)
                PlayerManager.Instance.AddStats(current.Negated());
            base.OnDisable();
        }

        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            var selfStats = PlayerStats.LerpLevel(statsLvl1, statsLvl10, lvl);
            return selfStats.PrintCompared(
                lvl == 0 || !withUpgrade
                    ? selfStats
                    : PlayerStats.LerpLevel(statsLvl1, statsLvl10, lvl - 1));
        }
    }
}