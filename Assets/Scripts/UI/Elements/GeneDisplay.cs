using Gameplay.Genes;
using TMPro;
using UnityEngine;

namespace UI.Elements
{
    public class GeneDisplay : MonoBehaviour
    {
        [SerializeField] protected TMP_Text[] geneTexts = new TMP_Text[3];
        
        public void UpdateGeneText(TrioGene trio, GeneType geneType) => UpdateGeneText(trio, (int) geneType);
        protected virtual void UpdateGeneText(TrioGene trio, int geneType) => geneTexts[geneType].text = trio.GetGene(geneType).ToString();

        public void UpdateTrioAsMedian(TrioGene trio)
        {
            geneTexts[0].text = $"~{trio.GetGene(0)}";
            geneTexts[1].text = $"~{trio.GetGene(1)}";
            geneTexts[2].text = $"~{trio.GetGene(2)}";
        }
        
        public void UpdateTrioText(TrioGene trio)
        {
            UpdateGeneText(trio, 0);
            UpdateGeneText(trio, 1);
            UpdateGeneText(trio, 2);
        }
    }
}