using Gameplay.Genes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Elements
{
    public class MutationsRerollButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private RerollGeneDisplay geneDisplay;
        [SerializeField] private GameObject tooltipGO;

        public void SetCost(TrioGene cost, TrioGene have)
        {
            geneDisplay.UpdateTrioText(cost);
            geneDisplay.UpdateAffordable(have, cost);
            button.interactable = cost.GetGene(0) <= have.GetGene(0) &&
                                  cost.GetGene(1) <= have.GetGene(1) &&
                                  cost.GetGene(2) <= have.GetGene(2);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipGO.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipGO.SetActive(false);
        }
    }
}