using Scriptable;

namespace UI
{
    public interface IAbilityTooltipProvider
    {
        public BasicMutation TooltipData { get; }
        public int Level { get; }
        public bool ShowUpgradeStats { get; }
    }
}