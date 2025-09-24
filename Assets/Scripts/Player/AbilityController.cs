using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Mutations;
using Scriptable;
using UI;
using UI.Elements;
using UI.Menus;
using UnityEngine;

namespace Player
{
    public class AbilityController : MonoBehaviour
    {
        private static AbilityController instance;

        [SerializeField] private GameObject uiGO;
        [SerializeField] private ActiveAbilityButton abilityButtonPrefab;
        [SerializeField] private BasicAbilityButton basicAbilityPrefab;
        [SerializeField] private Transform activeButtonsTransform;
        [SerializeField] private Transform basicButtonsTransform;
        [SerializeField] private List<BasicAbility> allAbilities = new();

        private AbilityController() => instance = this;
        private static readonly Dictionary<BasicMutation, BasicAbility> abilitiesDict = new();

        private void Awake()
        {
            MainMenu.OnResetRequested += OnResetRequested;
        }

        public static string GetAbilityLevelDescription(BasicMutation mutation, int lvl, bool withUpgrade) 
            => abilitiesDict[mutation].GetLevelDescription(lvl, withUpgrade);

        public void CreateUIElement(BasicAbility ability)
        {
            abilitiesDict[ability.Scriptable] = ability;
            if (ability is ActiveAbility activeAbility)
                Instantiate(abilityButtonPrefab, activeButtonsTransform)
                    .SetAbility(activeAbility);
            else Instantiate(basicAbilityPrefab, basicButtonsTransform)
                    .SetAbility(ability);
        }

        public static void UpdateAbilities(Egg egg) => instance.UpdateAbilitiesNonStatic(egg);
        
        private void UpdateAbilitiesNonStatic(Egg egg)
        {
            var eggAbilities = egg.MutationData.GetAll();
            foreach (BasicAbility ability in allAbilities)
            {
                var scriptable = ability.Scriptable;
                bool hasAbility = eggAbilities.ContainsKey(scriptable);
                ability.gameObject.SetActive(hasAbility);
                if (hasAbility)
                {
                    ability.SetLevel(eggAbilities[scriptable], true);
                }
            }
        }

        public static MutationData GetMutationData()
        {
            MutationData data = new MutationData();
            foreach (var basicAbility in instance.allAbilities.Where(basicAbility => basicAbility.Learned))
                data.Add(basicAbility.Scriptable, basicAbility.Level);
            return data;
        }

        private void OnResetRequested()
        {
            UpdateAbilitiesNonStatic(new Egg(TrioGene.Zero, new MutationData()));
        }

        private void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
        }

        public static void SetUIActive(bool isActive) => instance.uiGO.SetActive(isActive);
    }
}