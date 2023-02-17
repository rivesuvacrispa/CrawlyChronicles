using GameCycle;
using Genes;
using UnityEngine;

namespace UI
{
    public class EggBedViewerElement : MonoBehaviour
    {
        [SerializeField] private GeneDisplay display;
        [SerializeField] private RespawnManager respawnManager;

        public TrioGene TrioGene { get; private set; }

        public void Click()
        {
            respawnManager.SelectEggToRespawn(TrioGene);
        }

        public void SetGenes(TrioGene trioGene)
        {
            TrioGene = trioGene;
            display.UpdateTrioText(trioGene);
        }
    }
}