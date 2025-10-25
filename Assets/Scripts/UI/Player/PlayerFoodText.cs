using Gameplay.Breeding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class PlayerFoodText : MonoBehaviour
    {
        [SerializeField] private TMP_Text foodText;

        private void OnEnable()
        {
            OnFoodChanged(BreedingManager.Instance.CurrentFoodAmount,
                BreedingManager.Instance.CurrentBreedingFoodRequirement);
            BreedingManager.OnFoodChanged += OnFoodChanged;
        }

        private void OnDisable()
        {
            BreedingManager.OnFoodChanged -= OnFoodChanged;
        }

        private void OnFoodChanged(int current, int needed)
        {
            foodText.text = $"{current}/{needed}";
            foodText.color = current >= needed ? Color.green : Color.white;
        }
    }
}