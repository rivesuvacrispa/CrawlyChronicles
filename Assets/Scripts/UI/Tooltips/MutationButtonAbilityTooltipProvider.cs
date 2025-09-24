using UnityEngine.EventSystems;

namespace UI.Tooltips
{
    public class MutationButtonAbilityTooltipProvider : AbilityTooltipProvider
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            ((MutationAbilityTooltip) tooltip).ShowCost(true);
        }
    }
}