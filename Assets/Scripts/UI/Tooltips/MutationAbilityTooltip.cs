using Definitions;
using Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltips
{
    public class MutationAbilityTooltip : AbilityTooltip
    {
        [SerializeField] private Text costText;
        [SerializeField] private Image geneTypeImage;
        [SerializeField] private GameObject costGO;
        
        public override void SetTooltip(BasicMutation mutation, int lvl, bool withUpgrade)
        {
            costText.text = GlobalDefinitions.GetMutationCost(lvl).ToString();
            geneTypeImage.color = GlobalDefinitions.GetGeneColor(mutation.GeneType);
            base.SetTooltip(mutation, lvl, withUpgrade);
        }

        public void ShowCost(bool isActive) => costGO.SetActive(isActive);

    }
}