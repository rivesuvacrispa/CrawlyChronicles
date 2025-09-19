using Gameplay.Genes;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class RerollGeneDisplay : GeneDisplay
    {
        [SerializeField] private Image[] geneImages = new Image[3];
        [SerializeField] private float cantAffordAlpha;
        
        protected override void UpdateGeneText(TrioGene trio, int geneType)
        {
            Text text = geneTexts[geneType];
            int amount = trio.GetGene(geneType);
            text.text = amount.ToString();
            geneImages[geneType].gameObject.SetActive(amount > 0);
            text.gameObject.SetActive(amount > 0);
        }

        public void UpdateAffordable(TrioGene have, TrioGene need)
        {
            UpdateAffordable(0, have.GetGene(0) >= need.GetGene(0));
            UpdateAffordable(1, have.GetGene(1) >= need.GetGene(1));
            UpdateAffordable(2, have.GetGene(2) >= need.GetGene(2));
        }

        private void UpdateAffordable(int geneType, bool affordable)
        {
            var txt = geneTexts[geneType];
            var img = geneImages[geneType];
            txt.color = txt.color.WithAlpha(affordable ? 1f : cantAffordAlpha);
            img.color = img.color.WithAlpha(affordable ? 1f : cantAffordAlpha);
        }
    }
}