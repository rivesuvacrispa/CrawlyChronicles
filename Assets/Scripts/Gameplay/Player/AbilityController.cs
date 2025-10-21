using System.Collections.Generic;
using System.Linq;
using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Mutations;
using Scriptable;
using UI.Elements;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Player
{
    public class AbilityController : MonoBehaviour
    {
        private static AbilityController instance;

        [SerializeField] private GameObject uiGO;
        [SerializeField] private ActiveAbilityButton abilityButtonPrefab;
        [SerializeField] private BasicAbilityButton basicAbilityPrefab;
        [SerializeField] private Transform activeButtonsTransform;
        [SerializeField] private Transform basicButtonsTransform;
        
        private List<BasicAbility> allAbilities = new();
        private AbilityController() => instance = this;
        private static readonly Dictionary<BasicMutation, BasicAbility> AbilitiesDict = new();

        
        
        private void Awake()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            allAbilities = gameObject.GetComponentsInChildren<BasicAbility>(true).ToList();
            foreach (BasicAbility ability in allAbilities) 
                CreateButton(ability);
        }

        public static string GetAbilityLevelDescription(BasicMutation mutation, int lvl, bool withUpgrade) 
            => AbilitiesDict[mutation].GetLevelDescription(lvl, withUpgrade);

        private void CreateButton(BasicAbility ability)
        {
            AbilitiesDict[ability.Scriptable] = ability;
            BasicAbilityButton b = ability is ActiveAbility 
                ? Instantiate(abilityButtonPrefab, activeButtonsTransform) 
                : Instantiate(basicAbilityPrefab, basicButtonsTransform);
            
            b.SetAbility(ability);
            b.gameObject.SetActive(ability.isActiveAndEnabled);
        }

        public static void UpdateAbilities(Egg egg) => instance.UpdateAbilitiesNonStatic(egg);
        
        private void UpdateAbilitiesNonStatic(Egg egg)
        {
            var eggAbilities = egg.MutationData.GetAll();
            foreach (BasicAbility ability in allAbilities)
            {
                var scriptable = ability.Scriptable;
                bool hasAbility = eggAbilities.ContainsKey(scriptable);
                ability.gameObject.SetActive(hasAbility || (PlayerManager.Instance.GodMode && ability.isActiveAndEnabled));
                if (hasAbility)
                {
                    ability.SetLevel(eggAbilities[scriptable], true);
                }
            }
        }

        public static MutationData GetMutationData()
        {
            MutationData data = new MutationData();
            foreach (var basicAbility in instance.allAbilities.Where(basicAbility => basicAbility.isActiveAndEnabled))
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