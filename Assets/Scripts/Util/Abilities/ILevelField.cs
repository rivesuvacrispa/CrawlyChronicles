namespace Util.Abilities
{
    public interface ILevelField
    {
        public float AtLv(int lvl);
        public ILevelField UseFormatter(StatFormatter formatter);
        public StatFormatter Formatter { get; }
        public string FormatString(int currentIndex, int againstIndex);
    }
}