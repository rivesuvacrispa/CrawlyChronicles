using UnityEngine.EventSystems;

namespace UI
{
    public class BasicMutationButtonAbilityTooltipProvider : AbilityTooltipProvider
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            tooltip.ShowCost(false);
        }
    }
}