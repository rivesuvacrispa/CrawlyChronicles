using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.Abilities;
using UI;
using UnityEngine;

namespace Player
{
    public class AbilityController : MonoBehaviour
    {
        private static AbilityController instance;

        [SerializeField] private GameObject uiGO;
        [SerializeField] private AbilityButton abilityButtonPrefab;
        [SerializeField] private BasicAbilityButton basicAbilityPrefab;
        [SerializeField] private Transform activeButtonsTransform;
        [SerializeField] private Transform basicButtonsTransform;
        [SerializeField] private List<BasicAbility> allAbilities = new();

        private AbilityController() => instance = this;

        public void CreateUIElement(BasicAbility ability)
        {
            if (ability is ActiveAbility activeAbility)
                Instantiate(abilityButtonPrefab, activeButtonsTransform)
                    .SetAbility(activeAbility);
            else
                Instantiate(basicAbilityPrefab, basicButtonsTransform)
                    .SetAbility(ability);
        }

        public void UpdateAbilities(Egg egg)
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

        public static void SetUIActive(bool isActive) => instance.uiGO.SetActive(isActive);
    }
}