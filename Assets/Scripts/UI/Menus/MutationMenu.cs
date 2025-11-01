using System;
using System.Collections.Generic;
using System.Linq;
using Definitions;
using GameCycle;
using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Mutations;
using Gameplay.Player;
using Scriptable;
using UI.Elements;
using UI.Tooltips;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.Menus
{
    public class MutationMenu : MonoBehaviour
    {
        private static MutationMenu instance;


        [Header("Mutations transforms")] 
        [SerializeField] private List<Transform> mutationTypeToTransform;

        [Header("Other refs")]
        [SerializeField] private RespawnManager respawnManager;
        [SerializeField] private Transform newMutationsTransform;
        [SerializeField] private MutationAbilityTooltip tooltip;
        [SerializeField] private BasicAbilityButton basicAbilityButtonPrefab;
        [SerializeField] private MutationButton mutationButtonPrefab;
        [SerializeField] private GeneDisplay geneDisplay;
        [SerializeField] private List<BasicMutation> allMutations = new();
        [SerializeField] private MutationsRerollButton rerollButton;
        [Header("Settings")]
        [SerializeField] private int maxMutationsAmount = 20;
        [SerializeField] private Vector2Int randomAmountBounds = new(6, 12);
        [SerializeField] private float mutationRerollCost;

        
        private Egg hatchingEgg = new(TrioGene.Zero, new MutationData());
        private MutationTarget mutationTarget;
        private TrioGene genesLeft;
        private readonly Dictionary<BasicMutation, BasicAbilityButton> basicAbilityButtons = new();
        private Dictionary<BasicMutation, int> current = new();
        private TrioGene rerollCost;
        private readonly List<MutationButton> currentButtons = new();
        private float currentMutationRerollCost;


        public delegate void MutationMenuEvent();
        public static event MutationMenuEvent OnMutationClick;
        


        private MutationMenu() => instance = this;

        private void Awake() => SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;

        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;

        public static void Show(MutationTarget target, Egg egg) => instance.ShowNonStatic(target, egg);
        
        private void ShowNonStatic(MutationTarget target, Egg egg)
        {
            Time.timeScale = 0;
            mutationTarget = target;
            tooltip.Clear();
            hatchingEgg = egg;
            genesLeft = egg.Genes;
            geneDisplay.UpdateTrioText(genesLeft);
            current = egg.MutationData.GetAll();
            ShowCurrentMutations();
            UpdateRerollButton();
            CreateMutations();
            
            gameObject.SetActive(true);
        }

        private void CreateMutations()
        {
            // Dictionary of mutation + lvl of final given variants
            Dictionary<BasicMutation, int> variants = new();
            HashSet<BasicMutation> maxed = new();
            HashSet<BasicMutation> incompatible = new();
            
            // Build collection of incompatible mutations
            foreach (var (m, _) in current)
            {
                if (m.HasIncompatible) incompatible.AddRange(m.IncompatibleMutations);
            }

            // Build collection of mutations that player maxxed
            foreach (var (basicMutation, lvl) in current)
            {
                if (lvl == 9) maxed.Add(basicMutation);
            }
            
            // Build collection of all existing available mutations
            var availableSet = LinqUtility.ToHashSet(allMutations);
            // Remove maxed mutations from available collection
            availableSet.ExceptWith(maxed);
            // Remove incompatible mutations
            availableSet.ExceptWith(incompatible);
            var available = availableSet.ToList();

            float bonusChance = PlayerManager.PlayerStats.Mutagenicity;
            int mutationsAmount = Random.Range(randomAmountBounds.x, randomAmountBounds.y + 1);
            
            // For each rest possible mutation slot, add it according to mutagenicity value
            for (int i = 0; i < maxMutationsAmount - randomAmountBounds.y; i++)
                if (Random.value <= bonusChance)
                    mutationsAmount++;
            
            int len = available.Count;
            int amount = Mathf.Clamp(mutationsAmount, 1, Mathf.Min(maxMutationsAmount, len));
            
            // For a chosen amount of mutations
            while (amount > 0)
            {
                // Choose one random
                BasicMutation chosenOne = available[Random.Range(0, len)];
                
                // If mutation already in the list of given mutations
                if (variants.ContainsKey(chosenOne))
                {
                    // If list of given mutations equal to a list of possible mutations, there's no more available
                    if(variants.Count == len) break;
                    
                    continue;
                }

                // If player has chosen mutation, give it +1 lvl 
                int lvl = 0;
                if (current.ContainsKey(chosenOne)) 
                    lvl = current[chosenOne] + 1;
                
                // Add chosen mutation to a final variants list
                variants.Add(chosenOne, lvl);
                amount--;
            }
            
            // Create buttons for variants
            foreach (var (mutation, lvl) in variants)
            {
                var btn = Instantiate(mutationButtonPrefab, newMutationsTransform);
                btn.SetMutation(mutation, lvl);
                btn.GetComponent<AbilityTooltipProvider>().SetTooltip(tooltip);
                btn.SetAffordable(CanAfford(mutation, lvl));
                btn.OnClick = OnClick;
                currentButtons.Add(btn);
            }
        }

        private void ShowCurrentMutations()
        {
            foreach (var (mutation, level) in current) 
                CreateBasicAbilityButton(mutation, level);
        }

        private void OnClick(BasicMutation mutation, int level)
        {
            // Hide incompatible mutations of clicked one, if such exists
            if (mutation.HasIncompatible)
            {
                Debug.Log($"Click on {mutation.Name}");
                foreach (var button in 
                         currentButtons.Where(
                             button => button.Scriptable.HasIncompatible &&
                                       mutation.IncompatibleMutations.Contains(button.Scriptable)))
                {
                    Debug.Log($"Settings {button.Scriptable.Name} as unavailable");
                    button.SetUnavailable();
                }
            }
            
            int cost = GlobalDefinitions.GetMutationCost(level);
            
            if (current.ContainsKey(mutation))
            {
                current[mutation] = level;
                basicAbilityButtons[mutation].UpdateLevelText(level);
            }
            else
            {
                CreateBasicAbilityButton(mutation, level);
                current.Add(mutation, level);
            }

            genesLeft.SetGene(mutation.GeneType, genesLeft.GetGene(mutation.GeneType) - cost);
            geneDisplay.UpdateTrioText(genesLeft);
            RefreshAffordable();
            UpdateRerollButton();
            OnMutationClick?.Invoke();
        }

        private void CreateBasicAbilityButton(BasicMutation mutation, int level)
        {
            Transform t = mutationTypeToTransform[(int)mutation.GeneType];
            
            var btn = Instantiate(basicAbilityButtonPrefab, t);
            btn.SetVisuals(mutation);
            btn.UpdateLevelText(level);
            btn.GetComponent<AbilityTooltipProvider>().SetTooltip(tooltip);
            basicAbilityButtons.Add(mutation, btn);
        }

        private bool CanAfford(BasicMutation mutation, int level) =>
            genesLeft.GetGene(mutation.GeneType) >= GlobalDefinitions.GetMutationCost(level);

        private void RefreshAffordable()
        {
            foreach (Transform t in newMutationsTransform)
            {
                var btn = t.GetComponent<MutationButton>();
                btn.SetAffordable(CanAfford(btn.Scriptable, btn.Level));
            }
        }

        public void Hatch()
        {
            Egg mutated = new Egg(genesLeft, new MutationData(
                current.ToDictionary(
                    pair => pair.Key, 
                    pair => pair.Value)));
            gameObject.SetActive(false);
            ClearAll();
            if(mutationTarget is MutationTarget.Egg) 
                respawnManager.Respawn(hatchingEgg, mutated);
            else if (mutationTarget is MutationTarget.Player)
            {
                BreedingManager.Instance.SetTrioGene(mutated.Genes);
                AbilityController.UpdateAbilities(mutated);
                Time.timeScale = 1;
            }
        }
        
        public void Reroll()
        {
            genesLeft.SetGene(0, genesLeft.GetGene(0) - rerollCost.GetGene(0));
            genesLeft.SetGene(1, genesLeft.GetGene(1) - rerollCost.GetGene(1));
            genesLeft.SetGene(2, genesLeft.GetGene(2) - rerollCost.GetGene(2));
            geneDisplay.UpdateTrioText(genesLeft);
            foreach (Transform t in newMutationsTransform) 
                Destroy(t.gameObject);
            UpdateRerollButton();
            currentButtons.Clear();
            CreateMutations();
        }
        
        private void UpdateRerollButton()
        {
            rerollCost = genesLeft.AsRerollCost(currentMutationRerollCost);
            rerollButton.SetCost(rerollCost, genesLeft);
        }


        // Utils
        private void ClearAll()
        {
            foreach (Transform t in mutationTypeToTransform[0]) Destroy(t.gameObject);
            foreach (Transform t in mutationTypeToTransform[1]) Destroy(t.gameObject);
            foreach (Transform t in mutationTypeToTransform[2]) Destroy(t.gameObject);
            foreach (Transform t in newMutationsTransform) Destroy(t.gameObject);
            basicAbilityButtons.Clear();
            current.Clear();
            currentButtons.Clear();
        }
        
        private void OnDifficultyChanged(Difficulty difficulty)
        {
            currentMutationRerollCost = mutationRerollCost * difficulty.GeneRerollCost;
        }
    }

    public enum MutationTarget
    {
        Egg,
        Player
    }
}