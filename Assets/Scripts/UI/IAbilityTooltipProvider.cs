using Scriptable;

namespace UI
{
    public interface IAbilityTooltipProvider
    {
        public BasicMutation TooltipData { get; }
        int Level { get; }
    }
}