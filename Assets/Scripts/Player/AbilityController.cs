using Gameplay.Abilities;
using UI;
using UnityEngine;

namespace Player
{
    public class AbilityController : MonoBehaviour
    {
        [SerializeField] private AbilityButton abilityButtonPrefab;
        [SerializeField] private Transform buttonsTransform;

        public void CreateButton(Ability ability) => 
            Instantiate(abilityButtonPrefab, buttonsTransform)
                .SetAbility(ability);
    }
}