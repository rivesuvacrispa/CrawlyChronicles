using Gameplay.Breeding;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class PlayerTotalEggsText : MonoBehaviour
    {
        [SerializeField] private Text text;

        private void OnEnable()
        {
            BreedingManager.OnTotalEggsChanged += UpdateTotalEggsText;
        }

        private void OnDisable()
        {
            BreedingManager.OnTotalEggsChanged -= UpdateTotalEggsText;
        }

        private void UpdateTotalEggsText(int eggsAmount)
        {
            text.text = eggsAmount.ToString();
            text.color = eggsAmount == 0 ? Color.red : Color.white;
        }
    }
}