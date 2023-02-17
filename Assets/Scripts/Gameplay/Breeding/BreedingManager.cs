using System.Collections;
using System.Collections.Generic;
using Definitions;
using Gameplay.Genetics;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Gameplay
{
    public class BreedingManager : MonoBehaviour, INotificationProvider
    {
        public static BreedingManager Instance { get; private set; }

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
        [Header("Genes")] 
        [SerializeField] private Text aggressiveGenesAmountText;
        [SerializeField] private Text defensiveGenesAmountText;
        [SerializeField] private Text universalGenesAmountText;

        private int totalEggsAmount;
        private int currentFoodAmount;
        private TrioGene trioGene = TrioGene.Zero;
        private readonly Text[] geneTexts = new Text[3];
        private PopupNotification popupNotification;
        
        private readonly int breedAnimHash = Animator.StringToHash("PlayerBodyBreeding");
        private readonly int idleAnimHash = Animator.StringToHash("PlayerBodyIdle");

        private static int eggLayingTimer;
        public bool CanBreed => currentFoodAmount >= breedingFoodRequirement && eggLayingTimer == 0;
        
        private void Awake()
        {
            geneTexts[0] = aggressiveGenesAmountText;
            geneTexts[1] = defensiveGenesAmountText;
            geneTexts[2] = universalGenesAmountText;
            Instance = this;
            totalEggsAmount = 0;
            currentFoodAmount = 0;
            UpdateTotalEggsText();
            UpdateFoodText();
        }
        
        public void AddGene(GeneType geneType)
        {
            geneConsumer.ConsumeGene(geneType);
            trioGene.AddGene(geneType);
            UpdateGeneText(geneType);
        }
        
        private void LayEggs(Vector2 position)
        {
            var bed = Instantiate(eggBedPrefab, GlobalDefinitions.GameObjectsTransform);
            int amount = Random.Range(1, 7);
            var eggs = new List<TrioGene>();
            while (amount > 0)
            {
                eggs.Add(TrioGene.One);
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

        private void UpdateGeneText(GeneType geneType) => geneTexts[(int) geneType].text = trioGene.GetGene(geneType).ToString();
        
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

        public void BecomePregnant()
        {
            currentFoodAmount -= breedingFoodRequirement;
            eggLayingTimer = pregnancyDuration;
            StartCoroutine(EggLayRoutine());
        }

        private IEnumerator EggLayRoutine()
        {
            popupNotification = GlobalDefinitions.CreateNotification(this, false);
            
            while (eggLayingTimer > 0)
            {
                OnDataUpdate?.Invoke();
                yield return new WaitForSeconds(1f);
                eggLayingTimer--;
            }
            
            LayEggs(Player.Movement.Position);
            Destroy(popupNotification.gameObject);
            popupNotification = null;
        }

        public void Abort()
        {
            StopCoroutine(EggLayRoutine());
            eggLayingTimer = 0;
            if(popupNotification is not null) 
                Destroy(popupNotification.gameObject);
            popupNotification = null;
        }

        private void OnDestroy() => OnProviderDestroy?.Invoke();


        
        // INotificationProvider
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.NotificationProviderEvent OnProviderDestroy;
        public Transform Transform => Player.Movement.Transform;
        public string NotificationText => eggLayingTimer.ToString();
    }
}