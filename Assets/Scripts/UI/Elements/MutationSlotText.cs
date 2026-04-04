using Gameplay.Genes;
using TMPro;
using UnityEngine;

namespace UI.Elements
{
    public class MutationSlotText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private GeneType geneType;


        public void UpdateCanFit(TrioGene current, TrioGene max)
        {
            int c = current.GetGene(geneType);
            int m = max.GetGene(geneType);
            text.SetText($"{c}/{m}");
            text.color = c < m ? Color.white : Color.red;
        }
    }
}