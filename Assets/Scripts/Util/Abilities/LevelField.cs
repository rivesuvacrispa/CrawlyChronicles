using UnityEngine;

namespace Util.Abilities
{
    [System.Serializable]
    public abstract class LevelField<T, TJ> : ILevelField
    {
        [SerializeField] protected T value;

        public string TranslationKey { get; private set; } = string.Empty;
        public StatFormatter Formatter { get; private set; } = StatFormatter.DECIMAL;

        
        
        public LevelField(T v)
        {
            value = v;
        }

        public T Value => value;
        
        public abstract TJ AtLvl(int lvl);
        public float AtLv(int lvl) 
        {
            TJ result = AtLvl(lvl);
            return System.Convert.ToSingle(result);
        }

        public ILevelField UseKey(LevelFieldKey key)
        {
            TranslationKey = key.name;
            return this;
        }

        public ILevelField UseFormatter(StatFormatter f)
        {
            Formatter = f;
            return this;
        }
        
        public string FormatString(int currentIndex, int againstIndex)
        {
            return Formatter.FormatString(currentIndex, againstIndex, TranslationKey);
        }
    }
}