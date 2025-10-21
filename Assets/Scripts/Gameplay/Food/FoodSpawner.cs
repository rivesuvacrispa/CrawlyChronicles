using System.Collections;
using Gameplay.Map;
using Scriptable;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Food
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private int startingFoodAmount;
        [SerializeField] private float foodPerMinute;

        // Depends on difficulty
        private float currentFoodPerMinute;
        

        
        
        private void Awake()
        {
            SubToEvents();
        }
        
        private void Start()
        {
            OnDifficultyChanged(SettingsMenu.SelectedDifficulty);
            OnResetRequested();
            StartCoroutine(FoodSpawningRoutine());
        }

        private void OnResetRequested()
        {
            for (int i = 0; i < startingFoodAmount + (int) SettingsMenu.SelectedDifficulty.OverallDifficulty; i++)
                SpawnFood();
        }

        private void SpawnFood()
        {
            if (MapManager.TryGetFoodSpawnPoint(out var spawnPoint)) 
                spawnPoint.Spawn(MapManager.GetRandomFood());
        }

        private IEnumerator FoodSpawningRoutine()
        {
            while (currentFoodPerMinute > 0)
            {
                yield return new WaitForSeconds(1f);
                
                float delay = 60f / currentFoodPerMinute;
                delay -= delay * Random.value * 0.3f - 0.15f;
                yield return new WaitForSeconds(delay);
                
                SpawnFood();
            }
        }

        

        private void OnDifficultyChanged(Difficulty difficulty) => currentFoodPerMinute = foodPerMinute * difficulty.FoodSpawnRate;
        
        private void SubToEvents()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }
        
        private void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
            SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
        }
    }
}