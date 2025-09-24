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
        [SerializeField] private Transform specialsTransform;
        [SerializeField] private Transform passivesTransform;
        [SerializeField] private Transform activesTransform;

        [Header("Other refs")]
        [SerializeField] private RespawnManager respawnManager;
        [SerializeField] private Transform newMutationsTransform;
        [SerializeField] private MutationAbilityTooltip tooltip;
        [SerializeField] private BasicAbilityButton basicAbilityButtonPrefab;
        [SerializeField] private MutationButton mutationButtonPrefab;
        [SerializeField] private GeneDisplay geneDisplay;
        [SerializeField] private List<BasicMutation> allMutations = new();
        [SerializeField] private MutationsRerollButton rerollButton;

        [SerializeField] private Egg hatchingEgg = new(TrioGene.Zero, new MutationData());

        private MutationTarget mutationTarget;
        private TrioGene genesLeft;
        private readonly Dictionary<BasicMutation, BasicAbilityButton> basicAbilityButtons = new();
        private Dictionary<BasicMutation, int> current = new();
        private TrioGene rerollCost;

        public delegate void MutationMenuEvent();
        public static event MutationMenuEvent OnMutationClick;
        


        private MutationMenu() => instance = this;

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
            Dictionary<BasicMutation, int> variants = new();
            HashSet<BasicMutation> maxed = new();
            List<BasicMutation> specials = new();
            foreach (var (basicMutation, lvl) in current)
            {
                if (lvl == 9) maxed.Add(basicMutation);
                else if(basicMutation.Special) specials.Add(basicMutation);
            }
            var availableSet = LinqUtility.ToHashSet(allMutations.Where(m => !m.Special));
            availableSet.ExceptWith(maxed);
            var available = availableSet.ToList();
            available.AddRange(specials);

            int amount = Random.Range(6, 13);
            int len = available.Count;
            if (amount > len) amount = len;
            while (amount > 0)
            {
                BasicMutation chosenOne = available[Random.Range(0, len)];
                if (variants.ContainsKey(chosenOne))
                {
                    if(variants.Count == len) break;
                    continue;
                }

                int lvl = 0;
                if (current.ContainsKey(chosenOne)) 
                    lvl = current[chosenOne] + 1;
                variants.Add(chosenOne, lvl);
                amount--;
            }
            
            foreach (var (mutation, lvl) in variants)
            {
                var btn = Instantiate(mutationButtonPrefab, newMutationsTransform);
                btn.SetMutation(mutation, lvl);
                btn.GetComponent<AbilityTooltipProvider>().SetTooltip(tooltip);
                btn.SetAffordable(CanAfford(mutation, lvl));
                btn.OnClick = OnClick;
            }
        }

        private void ShowCurrentMutations()
        {
            foreach (var (mutation, level) in current) 
                CreateBasicAbilityButton(mutation, level);
        }

        private void OnClick(BasicMutation mutation, int level)
        {
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
            Transform t = mutation.Special ? specialsTransform :
                mutation is ActiveMutation ? activesTransform : passivesTransform;
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
            CreateMutations();
        }
        
        private void UpdateRerollButton()
        {
            rerollCost = genesLeft.AsRerollCost(BreedingManager.Instance.MutationRerollCost);
            rerollButton.SetCost(rerollCost, genesLeft);
        }


        // Utils
        private void ClearAll()
        {
            foreach (Transform t in specialsTransform) Destroy(t.gameObject);
            foreach (Transform t in passivesTransform) Destroy(t.gameObject);
            foreach (Transform t in activesTransform) Destroy(t.gameObject);
            foreach (Transform t in newMutationsTransform) Destroy(t.gameObject);
            basicAbilityButtons.Clear();
            current.Clear();
        }
    }

    public enum MutationTarget
    {
        Egg,
        Player
    }
}