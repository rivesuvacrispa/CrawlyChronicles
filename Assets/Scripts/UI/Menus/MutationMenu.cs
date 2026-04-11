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
using Util.Enums;
using Random = UnityEngine.Random;

namespace UI.Menus
{
    public class MutationMenu : MonoBehaviour
    {
        [Header("Mutations transforms")]
        [SerializeField] private List<Transform> mutationTypeToTransform;

        [Header("Other refs")]
        [SerializeField] private RespawnManager respawnManager;

        [SerializeField] private Transform newMutationsTransform;
        [SerializeField] private MutationAbilityTooltip tooltip;
        [SerializeField] private BasicAbilityButton basicAbilityButtonPrefab;
        [SerializeField] private MutationButton mutationButtonPrefab;
        [SerializeField] private GeneDisplay geneDisplay;
        [SerializeField] private MutationsRerollButton rerollButton;
        [SerializeField] private MutationSlotGroup slotGroup;


        private Egg hatchingEgg = new(TrioGene.Zero, new MutationData());
        private RespawnTarget respawnTarget;
        private TrioGene genesLeft;
        private readonly Dictionary<BasicMutation, BasicAbilityButton> basicAbilityButtons = new();
        private MutationData current = new();
        private TrioGene rerollCost;
        private readonly List<MutationButton> currentButtons = new();


        public delegate void MutationMenuEvent();

        public static event MutationMenuEvent OnMutationClick;


        public void Show(RespawnTarget target, Egg egg)
        {
            Time.timeScale = 0;
            respawnTarget = target;
            tooltip.Clear();
            hatchingEgg = egg;
            genesLeft = egg.Genes;
            geneDisplay.UpdateTrioText(genesLeft);
            current = egg.MutationData;
            CreateCurrentButtons();
            CreateFieldButtons(MutationManager.Instance.GetRandomVariants());
            UpdateRerollButton();

            gameObject.SetActive(true);
        }

        private void CreateFieldButtons(Dictionary<BasicMutation, int> variants)
        {
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

        private void CreateCurrentButtons()
        {
            foreach (var (mutation, level) in current.GetAll())
                CreateBasicAbilityButton(mutation, level);
        }

        private void OnClick(BasicMutation mutation, int level)
        {
            // Make incompatible mutations unavailable of clicked one, if such exists
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
            current.Set(mutation, level);

            if (basicAbilityButtons.ContainsKey(mutation))
                basicAbilityButtons[mutation].UpdateLevelText(level);
            else
                CreateBasicAbilityButton(mutation, level);

            genesLeft.SetGene(mutation.GeneType, genesLeft.GetGene(mutation.GeneType) - cost);
            geneDisplay.UpdateTrioText(genesLeft);
            TryBreakRandomMutation();
            UpdateAffordable();
            UpdateRerollButton();
            UpdateSlotsGroup();
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
            genesLeft.GetGene(mutation.GeneType) >= GlobalDefinitions.GetMutationCost(level) &&
            MutationManager.CurrentMutationData.CanFitMutation(mutation);

        private void UpdateAffordable()
        {
            foreach (Transform t in newMutationsTransform)
            {
                var btn = t.GetComponent<MutationButton>();
                btn.SetAffordable(CanAfford(btn.Scriptable, btn.Level));
            }
        }

        public void Hatch()
        {
            Egg mutated = new Egg(genesLeft, current.Copy());
            gameObject.SetActive(false);
            ClearAll();
            if (respawnTarget is RespawnTarget.Egg)
                respawnManager.Respawn(hatchingEgg, mutated);
            else if (respawnTarget is RespawnTarget.Player)
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
            CreateFieldButtons(MutationManager.Instance.GetRandomVariants());
        }

        private void UpdateRerollButton()
        {
            rerollCost = genesLeft.AsRerollCost(MutationManager.RerollCost);
            rerollButton.SetCost(rerollCost, genesLeft);
        }
        
        private void TryBreakRandomMutation()
        {
            if (!MutationManager.Instance.TryBreakRandomMutation(out BasicMutation broken, out int downgradedLvl) ||
                !basicAbilityButtons.TryGetValue(broken, out BasicAbilityButton b))
                return;

            if (downgradedLvl == -1)
            {
                basicAbilityButtons.Remove(broken);
                b.PlayBreak();
            }
            else
            {
                b.PlayDowngrade(downgradedLvl);
            }
        }

        private void UpdateSlotsGroup()
        {
            slotGroup.UpdateCanFit(current.CountTakenSlots(), CharacterManager.CurrentCharacter.MutationSlots);
        }


        // Utils
        private void ClearAll()
        {
            foreach (Transform t in mutationTypeToTransform[0]) Destroy(t.gameObject);
            foreach (Transform t in mutationTypeToTransform[1]) Destroy(t.gameObject);
            foreach (Transform t in mutationTypeToTransform[2]) Destroy(t.gameObject);
            foreach (Transform t in newMutationsTransform) Destroy(t.gameObject);
            basicAbilityButtons.Clear();
            currentButtons.Clear();
        }
    }
}