using System;
using Scriptable;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Bosses.PossessedAntColony
{
    [Serializable]
    public class PossessedAntColonyDefinitions : MonoBehaviour
    {
        private static PossessedAntColonyDefinitions instance;


        

        private void Awake()
        {
            OnDifficultyChanged(SettingsMenu.SelectedDifficulty);
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            
        }

        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;

        private PossessedAntColonyDefinitions() => instance = this;
    }
}