using System.Collections.Generic;
using Scriptable;
using UI;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EnemyDifficultyHandler : MonoBehaviour
    {
        [SerializeField] private List<Scriptable.Enemy> enemies = new();

        private void Awake() => SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;

        private void Start() => OnDifficultyChanged(SettingsMenu.SelectedDifficulty);

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            foreach (var enemy in enemies) 
                enemy.OnDifficultyChanged(difficulty);
        }

        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
    }
}