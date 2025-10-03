using System.Collections;
using System.Collections.Generic;
using Definitions;
using GameCycle;
using Gameplay.Genes;
using Gameplay.Map;
using Gameplay.Mutations;
using Gameplay.Mutations.Active;
using Gameplay.Player;
using Scriptable;
using UI;
using UI.Elements;
using UI.Menus;
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

        private PopupNotification popupNotification;
        private static int eggLayingTimer;
        private Coroutine eggLayRoutine;
        
        private float currentMutationRerollCost;
        public int CurrentFoodAmount { get; private set; }
        public int CurrentBreedingFoodRequirement { get; private set; }

        public static int TotalEggsAmount { get; private set; }
        [field:SerializeField] public TrioGene TrioGene { get; private set; } = TrioGene.Zero;
        public bool CanBreed => CurrentFoodAmount >= CurrentBreedingFoodRequirement && eggLayingTimer == 0;
        public float MutationRerollCost => currentMutationRerollCost;

        public delegate void TrioGeneEvent(TrioGene gene);
        public static event TrioGeneEvent OnTrioGeneChange;
        public delegate void EggsAmountEvent(int amount);
        public static event EggsAmountEvent OnTotalEggsChanged;
        public delegate void FoodChangeEvent(int current, int needed);
        public static event FoodChangeEvent OnFoodChanged;
        public delegate void BreedingEvent();
        public static event BreedingEvent OnBecomePregnant;
        public static event BreedingEvent OnAbortion;
        public static event EggsAmountEvent OnEggsLaid;
        
        
        
        
        private BreedingManager() => Instance = this;

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
            CreateFirstEggBed();
        }

        public void SetCurrentFoodAmount(int newAmount)
        {
            CurrentFoodAmount = newAmount;
            OnFoodChanged?.Invoke(CurrentFoodAmount, CurrentBreedingFoodRequirement);
        }
        
        private void SetTotalEggsAmount(int newAmount)
        {
            TotalEggsAmount = newAmount;
            OnTotalEggsChanged?.Invoke(TotalEggsAmount);
        }
        
        private void AddGene(GeneType geneType, int amount)
        {
            TrioGene.AddGene(geneType, amount);
            OnTrioGeneChange?.Invoke(TrioGene);
        }

        public void SetTrioGene(TrioGene trioGene)
        {
            TrioGene = trioGene;
            OnTrioGeneChange?.Invoke(TrioGene);
        }
        
        public void AddTotalEggsAmount(int amount)
        {
            TotalEggsAmount += amount;
            OnTotalEggsChanged?.Invoke(TotalEggsAmount);
        }

        public bool AddFood()
        {
            if (CurrentFoodAmount >= CurrentBreedingFoodRequirement) return false;
            CurrentFoodAmount++;
            OnFoodChanged?.Invoke(CurrentFoodAmount, CurrentBreedingFoodRequirement);
            return true;
        }
        
        public void BecomePregnant(TrioGene genes, MutationData mutationData)
        {
            OnBecomePregnant?.Invoke();
            CurrentFoodAmount -= CurrentBreedingFoodRequirement;
            eggLayingTimer = pregnancyDuration;
            eggLayRoutine = StartCoroutine(EggLayRoutine(genes, mutationData));
            OnFoodChanged?.Invoke(CurrentFoodAmount, CurrentBreedingFoodRequirement);
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
            
            LayEggs(PlayerMovement.Position, genes, mutationData);
            Destroy(popupNotification.gameObject);
            popupNotification = null;
            eggLayRoutine = null;
        }
        
        public void LayEggs(Vector2 position, TrioGene genes, MutationData mutationData)
        {
            var bed = Instantiate(GlobalDefinitions.EggBedPrefab, MapManager.GameObjectsTransform);
            int amount = Mathf.RoundToInt(Random.Range(Mathf.Clamp(2f + QueensFertility.EggsAmount, 2f, 6f), 6f));
            var eggs = new List<Egg>();
            OnEggsLaid?.Invoke(amount);

            
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
            OnAbortion?.Invoke();
            eggLayRoutine = null;
            eggLayingTimer = 0;
            if(popupNotification is not null) 
                Destroy(popupNotification.gameObject);
            popupNotification = null;
        }
        
        private void OnDifficultyChanged(Difficulty difficulty)
        {
            currentMutationRerollCost = mutationRerollCost * difficulty.GeneRerollCost;
            CurrentBreedingFoodRequirement = Mathf.FloorToInt(breedingFoodRequirement * difficulty.BreedingCost);
            OnFoodChanged?.Invoke(CurrentFoodAmount, CurrentBreedingFoodRequirement);
        }
        
        private void CreateFirstEggBed()
        {
            var bed = Instantiate(GlobalDefinitions.EggBedPrefab, MapManager.GameObjectsTransform);
            int amount = Random.Range(1, 7);
            var eggs = new List<Egg>();
            while (amount > 0)
            {
                Egg egg = new Egg(TrioGene.Zero, new MutationData());
                eggs.Add(egg);
                amount--;
            }

            OnEggsLaid?.Invoke(amount);
            bed.SetEggs(eggs);
            bed.transform.position = new Vector3(15, 15, 0);
        }

        private void SubToEvents()
        {
            GeneDrop.OnPickedUp += AddGene;
            MainMenu.OnResetRequested += OnResetRequested;
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }

        private void OnDestroy()
        {
            OnProviderDestroy?.Invoke(this);
            GeneDrop.OnPickedUp -= AddGene;
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