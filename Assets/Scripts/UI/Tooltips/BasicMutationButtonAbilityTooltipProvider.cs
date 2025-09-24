using UnityEngine.EventSystems;

namespace UI.Tooltips
{
    public class BasicMutationButtonAbilityTooltipProvider : AbilityTooltipProvider
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            ((MutationAbilityTooltip) tooltip).ShowCost(false);
        }
    }
}