using UnityEngine.EventSystems;

namespace UI
{
    public class MutationButtonAbilityTooltipProvider : AbilityTooltipProvider
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            tooltip.ShowCost(true);
        }
    }
}