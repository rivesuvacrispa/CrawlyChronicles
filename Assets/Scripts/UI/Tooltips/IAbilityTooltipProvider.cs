using Scriptable;

namespace UI.Tooltips
{
    public interface IAbilityTooltipProvider
    {
        public BasicMutation TooltipData { get; }
        public int Level { get; }
        public bool ShowUpgradeStats { get; }
    }
}