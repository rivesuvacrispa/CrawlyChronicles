using Gameplay.Breeding;
using Gameplay.Enemies.Enemies;
using Gameplay.Genes;
using Player;
using UI.Elements;
using UnityEngine;

namespace UI.Menus
{
    public class BreedingMenu : MonoBehaviour
    {
        [SerializeField] private GeneDisplay playerDisplay;
        [SerializeField] private GeneDisplay partnerDisplay;
        [SerializeField] private GeneDisplay eggsDisplay;

        private NeutralAnt openedPartner;
        private TrioGene median;
        private static BreedingMenu instance;


        private BreedingMenu() => instance = this;
        public static void OpenBreedingMenu(NeutralAnt partner) => instance.Open(partner);

        
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
            BreedingManager.Instance.BecomePregnant(median, AbilityController.GetMutationData());
            Close();
        }
    }
}