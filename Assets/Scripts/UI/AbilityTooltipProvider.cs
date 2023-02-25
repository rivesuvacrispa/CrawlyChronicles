using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class AbilityTooltipProvider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Component tooltipProvider;

        protected AbilityTooltip tooltip;

        private void Awake()
        {
            if (tooltip is null) enabled = false;
        }

        public void SetTooltip(AbilityTooltip newTooltip)
        {
            tooltip = newTooltip;
            enabled = true;
        }
        

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            var provider = (IAbilityTooltipProvider) tooltipProvider;
            transform.localScale = Vector3.one * 1.15f;
            tooltip.SetTooltip(provider.TooltipData, provider.Level);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
            tooltip.Clear();
        }
    }
}