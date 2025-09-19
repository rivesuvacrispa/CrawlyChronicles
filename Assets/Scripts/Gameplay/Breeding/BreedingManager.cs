using System.Collections;
using System.Collections.Generic;
using Definitions;
using GameCycle;
using Gameplay.Enemies.Enemies;
using Gameplay.Genes;
using Gameplay.Mutations;
using Gameplay.Mutations.Active;
using Player;
using Scriptable;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Breeding
{
    public class BreedingManager : MonoBehaviour, INotificationProvider
    {
        public static BreedingManager Instance { get; private set; }
        [Header("Definitions")]
        [SerializeField] private float mutationRerollCost;
        [SerializeField] private int breedingFoodRequirement;
        [SerializeField] private int pregnancyDuration = 5;
        [Header("Refs")]
        [SerializeField] private BreedingMenu breedingMenu;
        [SerializeField] private GeneDisplay geneDisplay;
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem breedingParticles;
        [SerializeField] private GeneConsumer geneConsumer;
        [SerializeField] private Text totalEggsText;
        [SerializeField] private Text foodText;

        private int currentFoodAmount;
        private PopupNotification popupNotification;
        private static int eggLayingTimer;
        private Coroutine eggLayRoutine;

        private readonly int breedAnimHash = Animator.StringToHash("PlayerBodyBreeding");
        private readonly int idleAnimHash = Animator.StringToHash("PlayerBodyIdle");

        // Depends on difficulty
        private float currentMutationRerollCost;
        private int currentBreedingFoodRequirement;

        public static int TotalEggsAmount { get; private set; }
        [field:SerializeField] public TrioGene TrioGene { get; private set; } = TrioGene.Zero;
        public bool CanBreed => currentFoodAmount >= currentBreedingFoodRequirement && eggLayingTimer == 0;
        public float MutationRerollCost => currentMutationRerollCost;
        
        
        
        private void Awake() => Instance = this;

        private void Start()
        {
            OnDifficultyChanged(SettingsMenu.SelectedDifficulty);
            SetCurrentFoodAmount(0);
            SetTotalEggsAmount(0);
            SubToEvents();
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
            TotalEggsAmount = newAmount;
            UpdateTotalEggsText();
        }
        
        public void AddGene(GeneType geneType, int amount)
        {
            geneConsumer.ConsumeGene(geneType);
            TrioGene.AddGene(geneType, amount);
            geneDisplay.UpdateGeneText(TrioGene, geneType);
        }

        public void SetTrioGene(TrioGene trioGene)
        {
            TrioGene = trioGene;
            geneDisplay.UpdateTrioText(trioGene);
        }
        
        private void UpdateTotalEggsText()
        {
            totalEggsText.text = TotalEggsAmount.ToString();
            totalEggsText.color = TotalEggsAmount == 0 ? Color.red : Color.white;
        }
        
        private void UpdateFoodText()
        {
            foodText.text = $"{currentFoodAmount}/{currentBreedingFoodRequirement}";
            foodText.color = currentFoodAmount >= currentBreedingFoodRequirement ? Color.green : Color.white;
        }

        
        public void AddTotalEggsAmount(int amount)
        {
            TotalEggsAmount += amount;
            UpdateTotalEggsText();
        }

        public bool AddFood()
        {
            if (currentFoodAmount >= currentBreedingFoodRequirement) return false;
            currentFoodAmount++;
            UpdateFoodText();
            return true;
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
            currentFoodAmount -= currentBreedingFoodRequirement;
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
            LayEggs(PlayerMovement.Position, genes, mutationData);
            Destroy(popupNotification.gameObject);
            popupNotification = null;
            eggLayRoutine = null;
        }
        
        public void LayEggs(Vector2 position, TrioGene genes, MutationData mutationData)
        {
            var bed = Instantiate(GlobalDefinitions.EggBedPrefab, GlobalDefinitions.GameObjectsTransform);
            int amount = Mathf.RoundToInt(Random.Range(Mathf.Clamp(2f + QueensFertility.EggsAmount, 2f, 6f), 6f));
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
        
        private void OnDifficultyChanged(Difficulty difficulty)
        {
            currentMutationRerollCost = mutationRerollCost * difficulty.GeneRerollCost;
            currentBreedingFoodRequirement = Mathf.FloorToInt(breedingFoodRequirement * difficulty.BreedingCost);
            UpdateFoodText();
        }

        private void SubToEvents()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }

        private void OnDestroy()
        {
            OnProviderDestroy?.Invoke();
            MainMenu.OnResetRequested -= OnResetRequested;
            SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
        }


        // INotificationProvider
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => PlayerManager.Instance.Transform;
        public string NotificationText => eggLayingTimer.ToString();
    }
}