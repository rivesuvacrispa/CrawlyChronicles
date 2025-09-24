using System.Collections.Generic;
using Scriptable;
using Timeline;
using UI;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EnemyDifficultyHandler : MonoBehaviour
    {
        [SerializeField] private List<Scriptable.Enemy> enemies = new();

        private void Awake()
        {
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
            TimeManager.OnDayStart += OnDayStart;
        }

        private void Start()
        {
            OnDifficultyChanged(SettingsMenu.SelectedDifficulty);
        }

        private void OnDayStart(int dayCounter)
        {
            foreach (var enemy in enemies) 
                enemy.OnDayChanged(dayCounter);
        }

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            foreach (var enemy in enemies) 
                enemy.OnDifficultyChanged(difficulty);
        }

        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
    }
}