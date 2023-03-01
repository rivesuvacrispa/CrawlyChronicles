using Scriptable;
using UnityEngine;
using Util;

namespace Scripts.Gameplay.Bosses
{
    [System.Serializable]
    public abstract class BossStatField<T>
    {
        [SerializeField] private BossStat<T>[] values = {
            new(BossDifficulty.Helpless),
            new(BossDifficulty.Default),
            new(BossDifficulty.Unfair),
        };

        public T CurrentValue { get; private set; }

        public void ChangeDifficulty(Difficulty difficulty) => CurrentValue = values[(int)difficulty.OverallDifficulty].Value;

        
        
        [System.Serializable]
        private struct BossStat<TJ>
        {
            [SerializeField, ShowOnly] private BossDifficulty difficulty;
            [SerializeField] private TJ value;

            public TJ Value => value;
        
            public BossStat(BossDifficulty difficulty) : this() => this.difficulty = difficulty;
        }
    }
}