using System.Collections;
using System.Collections.Generic;
using Definitions;
using GameCycle;
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

        [field:SerializeField] public TrioGene TrioGene { get; private set; } = TrioGene.Zero;
        public bool CanBreed => currentFoodAmount >= breedingFoodRequirement && eggLayingTimer == 0;
        
        
        
        private void Awake()
        {
            Instance = this;
            SetCurrentFoodAmount(0);
            SetTotalEggsAmount(0);
            MainMenu.OnResetRequested += OnResetRequested;
        }

        private void OnResetRequested()
        {
            StopAllCoroutines();
            eggLayingTimer = 0;
            SetCurrentFoodAmount(0);
            SetTotalEggsAmount(0);
            SetTrioGene(TrioGene.Zero);
        }

        public void SetCurrentFoodAmount(int newAmount)
        {
            currentFoodAmount = newAmount;
            UpdateFoodText();
        }
        
        private void SetTotalEggsAmount(int newAmount)
        {
            totalEggsAmount = newAmount;
            UpdateTotalEggsText();
        }
        
        public void AddGene(GeneType geneType)
        {
            geneConsumer.ConsumeGene(geneType);
            TrioGene.AddGene(geneType);
            geneDisplay.UpdateGeneText(TrioGene, geneType);
        }

        public void SetTrioGene(TrioGene trioGene)
        {
            TrioGene = trioGene;
            geneDisplay.UpdateTrioText(trioGene);
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
            Debug.Log($"paused: {breedingParticles.isPaused}, playing: {breedingParticles.isPlaying}");
        }

        public void PlayIdleAnimation()
        {
            animator.Play(idleAnimHash);
            breedingParticles.Stop();
        }

        public void BecomePregnant(TrioGene genes, MutationData mutationData)
        {
            StatRecorder.timesBreed++;
            breedingParticles.Play();
            currentFoodAmount -= breedingFoodRequirement;
            eggLayingTimer = pregnancyDuration;
            eggLayRoutine = StartCoroutine(EggLayRoutine(genes, mutationData));
            UpdateFoodText();
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
        
        private void LayEggs(Vector2 position, TrioGene genes, MutationData mutationData)
        {
            var bed = Instantiate(GlobalDefinitions.EggBedPrefab, GlobalDefinitions.GameObjectsTransform);
            int amount = Random.Range(3, 7);
            StatRecorder.eggsLayed += amount;
            var eggs = new List<Egg>();
            while (amount > 0)
            {
                Egg egg = new Egg(
                    genes.Randomize(GlobalDefinitions.EggGeneEntropy), 
                    mutationData.Randomize());
                eggs.Add(egg);
                amount--;
            }

            bed.SetEggs(eggs);
            bed.transform.position = position;
        }

        public void Abort()
        {
            if(eggLayRoutine is null) return;

            StopCoroutine(eggLayRoutine);
            breedingParticles.Stop();
            eggLayRoutine = null;
            eggLayingTimer = 0;
            if(popupNotification is not null) 
                Destroy(popupNotification.gameObject);
            popupNotification = null;
        }

        public void OpenBreedingMenu(NeutralAnt partner)
        {
            breedingMenu.Open(partner);
        }

        private void OnDestroy()
        {
            OnProviderDestroy?.Invoke();
            MainMenu.OnResetRequested -= OnResetRequested;
        }


        // INotificationProvider
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.NotificationProviderEvent OnProviderDestroy;
        public Transform Transform => Player.Movement.Transform;
        public string NotificationText => eggLayingTimer.ToString();
    }
}