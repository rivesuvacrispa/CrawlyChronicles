using Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MinimapDifficultyIcon : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite[] icons = new Sprite[3];

        private void OnDifficultyChanged(Difficulty difficulty) 
            => image.sprite = icons[(int) difficulty.OverallDifficulty];

        private void Start()
        {
            OnDifficultyChanged(SettingsMenu.SelectedDifficulty);
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }

        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
    }
}