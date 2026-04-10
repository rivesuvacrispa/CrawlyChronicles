using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Breeding;
using Gameplay.Mutations;
using Scriptable;
using UI.Menus;
using Unity.VisualScripting;
using UnityEngine;
using Util;
using Util.Enums;
using Random = UnityEngine.Random;

namespace Gameplay.Player
{
    public class MutationManager : MonoBehaviour
    {
        public static MutationManager Instance { get; private set; }

        [Header("Refs")]
        [SerializeField] private MutationMenu mutationMenu;

        [Header("Settings")]
        [SerializeField] private int maxMutationsAmount = 20;

        [SerializeField] private Vector2Int randomAmountBounds = new(6, 12);
        [SerializeField] private float mutationRerollCost = 0.2f;
        [SerializeField] private List<BasicMutation> obtainableMutations = new();

        public static float RerollCost { get; private set; }
        public static MutationData CurrentMutationData { private set; get; }

        public delegate void MutationBreakChanceEvent(FloatWrapper chance);

        public static event MutationBreakChanceEvent OnCollectBreakChance;


        private MutationManager() => Instance = this;

        public void MutateFromEgg(Egg egg)
        {
            ShowMenu(RespawnTarget.Egg, egg);
        }

        public void MutateFromPlayer()
        {
            ShowMenu(
                RespawnTarget.Player,
                new Egg(BreedingManager.Instance.TrioGene, AbilityController.GetMutationData())
            );
        }

        private void ShowMenu(RespawnTarget target, Egg egg)
        {
            CurrentMutationData = egg.MutationData;
            mutationMenu.Show(target, egg);
        }

        private void Awake()
        {
            RerollCost = mutationRerollCost;
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }

        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            RerollCost = mutationRerollCost * difficulty.GeneRerollCost;
        }

        private int GetRandomMutationAmount()
        {
            float bonusChance = PlayerManager.PlayerStats.Mutagenicity;
            int amount = Random.Range(randomAmountBounds.x, randomAmountBounds.y + 1);

            /*
             * For each rest possible mutation slot, try to add it according to mutagenicity value
             * The further the slot, the more mutagenicity it requires
             * 1st slot - 0-100%
             * 2nd slot - 100-200%
             * 3rd slot - 200-300%
             * etc.
             */
            for (int i = 0; i < maxMutationsAmount - randomAmountBounds.y; i++)
                if (i + Random.value <= bonusChance)
                    amount++;

            return amount;
        }

        public Dictionary<BasicMutation, int> GetRandomVariants()
        {
            // TODO: remove ones that are out of slots
            // Dictionary of mutation + lvl of final given variants
            Dictionary<BasicMutation, int> variants = new();
            HashSet<BasicMutation> maxed = new();
            HashSet<BasicMutation> incompatible = new();

            // Build collection of incompatible mutations
            var all = CurrentMutationData.GetAll();
            foreach (var (m, _) in all)
            {
                if (m.HasIncompatible) incompatible.AddRange(m.IncompatibleMutations);
            }

            // Build collection of mutations that player maxxed
            foreach (var (basicMutation, lvl) in all)
            {
                if (lvl == 9) maxed.Add(basicMutation);
            }

            // Build collection of all existing available mutations
            var availableSet = LinqUtility.ToHashSet(obtainableMutations);
            // Remove maxed mutations from available collection
            availableSet.ExceptWith(maxed);
            // Remove incompatible mutations
            availableSet.ExceptWith(incompatible);
            var available = availableSet.ToList();

            // Decide amount of mutations
            int amount = GetRandomMutationAmount();
            int len = available.Count;
            amount = Mathf.Clamp(amount, 1, Mathf.Min(amount, len));

            // For a chosen amount of mutations
            while (amount > 0)
            {
                // Choose one random
                BasicMutation chosenOne = available[Random.Range(0, len)];

                // If mutation already in the list of given mutations
                if (variants.ContainsKey(chosenOne))
                {
                    // If list of given mutations equal to a list of possible mutations, there's no more available
                    if (variants.Count == len) break;

                    continue;
                }

                // If player has chosen mutation, give it +1 lvl 
                int lvl = 0;
                if (all.ContainsKey(chosenOne))
                    lvl = all[chosenOne] + 1;

                // Add chosen mutation to a final variants list
                variants.Add(chosenOne, lvl);
                amount--;
            }

            return variants;
        }

        public bool TryBreakRandomMutation(out BasicMutation broken, out int downgradedLvl)
        {
            downgradedLvl = -1;
            broken = null;
            FloatWrapper breakChance = new FloatWrapper(0);
            OnCollectBreakChance?.Invoke(breakChance);
            if (Random.value >= breakChance.Value) return false;
            
            var candidates = new List<BasicMutation>();

            foreach (var (mutation, lvl) in CurrentMutationData.GetAll())
            {
                if (lvl == 9) continue;
                candidates.Add(mutation);
            }

            var candidate = candidates.OrderBy(_ => Random.value).FirstOrDefault();
            if (candidate is null ||
                !CurrentMutationData.TryGet(candidate, out int currentLvl))
                return false;

            downgradedLvl = currentLvl - 1;
            broken = candidate;

            Debug.Log($"Mutation {candidate.Name} is broken to lvl {downgradedLvl}");

            if (downgradedLvl == -1 && CurrentMutationData.Remove(candidate))
                return true;

            CurrentMutationData.Set(candidate, downgradedLvl);
            return true;
        }
    }
}