using System.Collections;
using System.Collections.Generic;
using Definitions;
using Gameplay.Abilities;
using Gameplay.Enemies;
using Genes;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Gameplay
{
    public class BreedingManager : MonoBehaviour, INotificationProvider
    {
        public static BreedingManager Instance { get; private set; }

        [SerializeField] private BreedingMenu breedingMenu;
        [SerializeField] private GeneDisplay geneDisplay;
        [SerializeField] private int pregnancyDuration = 5;
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem breedingParticles;
        [SerializeField] private GeneConsumer geneConsumer;
        [Header("Eggs")]
        [SerializeField] private Text totalEggsText;
        [SerializeField] private EggBed eggBedPrefab;
        [Header("Food")]
        [SerializeField] private Text foodText;
        [SerializeField] private int breedingFoodRequirement;

        private int totalEggsAmount;
        private int currentFoodAmount;
        private PopupNotification popupNotification;
        private static int eggLayingTimer;
        private Coroutine eggLayRoutine;

        private readonly int breedAnimHash = Animator.StringToHash("PlayerBodyBreeding");
        private readonly int idleAnimHash = Animator.StringToHash("PlayerBodyIdle");

        [field:SerializeField] public TrioGene TrioGene { get; set; } = TrioGene.Zero;
        public bool CanBreed => currentFoodAmount >= breedingFoodRequirement && eggLayingTimer == 0;
        public bool CanRespawn => totalEggsAmount > 0;
        
        
        
        private void Awake()
        {
            Instance = this;
            totalEggsAmount = 0;
            currentFoodAmount = 0;
            UpdateTotalEggsText();
            UpdateFoodText();
        }
        
        public void AddGene(GeneType geneType)
        {
            geneConsumer.ConsumeGene(geneType);
            TrioGene.AddGene(geneType);
            geneDisplay.UpdateGeneText(TrioGene, geneType);
        }

        public void UpdateGeneText()
        {
            geneDisplay.UpdateTrioText(TrioGene);
        }
        
        private void LayEggs(Vector2 position, TrioGene genes, MutationData mutationData)
        {
            var bed = Instantiate(eggBedPrefab, GlobalDefinitions.GameObjectsTransform);
            int amount = Random.Range(1, 7);
            var eggs = new List<Egg>();
            while (amount > 0)
            {
                Egg egg = new Egg(
                    genes.Randomize(GlobalDefinitions.EggGeneEntropy), 
                    mutationData.Randomize());
                eggs.Add(egg);
                amount--;
            }
            
            bed.AddEggs(eggs);
            bed.transform.position = position;
            UpdateFoodText();
        }

        private void UpdateTotalEggsText()
        {
            totalEggsText.text = totalEggsAmount.ToString();
            totalEggsText.color = totalEggsAmount == 0 ? Color.red : Color.white;
        }
        
        private void UpdateFoodText()
        {
            foodText.text = $"{currentFoodAmount}/{breedingFoodRequirement}";
            foodText.color = currentFoodAmount >= breedingFoodRequirement ? Color.green : Color.white;
        }

        
        public void AddTotalEggsAmount(int amount)
        {
            totalEggsAmount += amount;
            UpdateTotalEggsText();
        }

        public void AddFood()
        {
            currentFoodAmount++;
            UpdateFoodText();
        }

        public void PlayBreedingAnimation()
        {
            animator.Play(breedAnimHash);
            breedingParticles.Play();
        }

        public void PlayIdleAnimation()
        {
            animator.Play(idleAnimHash);
            breedingParticles.Stop();
        }

        public void BecomePregnant(TrioGene genes, MutationData mutationData)
        {
            breedingParticles.Play();
            currentFoodAmount -= breedingFoodRequirement;
            eggLayingTimer = pregnancyDuration;
            eggLayRoutine = StartCoroutine(EggLayRoutine(genes, mutationData));
        }

        private IEnumerator EggLayRoutine(TrioGene genes, MutationData mutationData)
        {
            popupNotification = GlobalDefinitions.CreateNotification(this, false);
            
            while (eggLayingTimer > 0)
            {
                OnDataUpdate?.Invoke();
                yield return new WaitForSeconds(1f);
                eggLayingTimer--;
            }
            
            breedingParticles.Stop();
            LayEggs(Player.Movement.Position, genes, mutationData);
            Destroy(popupNotification.gameObject);
            popupNotification = null;
            eggLayRoutine = null;
        }

        public void Abort()
        {
            if(eggLayRoutine is null) return;

            StopCoroutine(eggLayRoutine);
            breedingParticles.Stop();
            eggLayRoutine = null;
            popupNotification = null;
            eggLayingTimer = 0;
            if(popupNotification is not null) 
                Destroy(popupNotification.gameObject);
        }

        public void OpenBreedingMenu(NeutralAnt partner)
        {
            breedingMenu.Open(partner);
        }

        private void OnDestroy() => OnProviderDestroy?.Invoke();


        
        // INotificationProvider
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.NotificationProviderEvent OnProviderDestroy;
        public Transform Transform => Player.Movement.Transform;
        public string NotificationText => eggLayingTimer.ToString();
    }
}