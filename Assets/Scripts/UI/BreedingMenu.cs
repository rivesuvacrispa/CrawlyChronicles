using GameCycle;
using Gameplay;
using Gameplay.Enemies;
using Genes;
using Player;
using UnityEngine;

namespace UI
{
    public class BreedingMenu : MonoBehaviour
    {
        [SerializeField] private GeneDisplay playerDisplay;
        [SerializeField] private GeneDisplay partnerDisplay;
        [SerializeField] private GeneDisplay eggsDisplay;

        private NeutralAnt openedPartner;
        private TrioGene median;
        
        public void Open(NeutralAnt partner)
        {
            openedPartner = partner;
            TrioGene partnerGene = partner.TrioGene;
            TrioGene playerGene = BreedingManager.Instance.TrioGene;
            median = TrioGene.Median(playerGene, partnerGene);
            playerDisplay.UpdateTrioText(playerGene);
            partnerDisplay.UpdateTrioText(partnerGene);
            eggsDisplay.UpdateTrioAsMedian(median);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            openedPartner = null;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            AbilityController.SetUIActive(false);
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            AbilityController.SetUIActive(true);
            Time.timeScale = 1;
        }

        public void Accept()
        {
            openedPartner.CanBreed = false;
            StatRecorder.timesBreed++;
            BreedingManager.Instance.BecomePregnant(median, AbilityController.GetMutationData());
            Close();
        }
    }
}