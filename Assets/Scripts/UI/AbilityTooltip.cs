using Definitions;
using Player;
using Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AbilityTooltip : MonoBehaviour
    {
        [SerializeField] private GameObject rootGO;
        [SerializeField] private MutationButton button;
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text costText;
        [SerializeField] private Image geneTypeImage;
        [SerializeField] private GameObject costGO;
        
        public void SetTooltip(BasicMutation mutation, int lvl)
        {
            button.SetMutation(mutation, lvl);
            nameText.text = mutation.Name;
            descriptionText.text = mutation.Description + "\n\n" + AbilityController
                .GetAbilityLevelDescription(mutation, lvl);
            costText.text = GlobalDefinitions.GetMutationCost(lvl).ToString();
            geneTypeImage.color = GlobalDefinitions.GetGeneColor(mutation.GeneType);
            rootGO.SetActive(true);
        }

        public void ShowCost(bool isActive) => costGO.SetActive(isActive);

        public void Clear() => rootGO.SetActive(false);
    }
}