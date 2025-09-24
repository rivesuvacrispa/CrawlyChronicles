using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scriptable;
using UI;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Food
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private int startingFoodAmount;
        [SerializeField] private float foodPerMinute;
        [SerializeField] private List<Foodbed> foodbeds;
        [SerializeField] private Transform foodSpawnPointsTransform;

        // Depends on difficulty
        private float currentFoodPerMinute;
        
        private readonly List<FoodSpawnPoint> foodSpawnPoints = new();

        
        
        private void Awake()
        {
            foreach (Transform child in foodSpawnPointsTransform)
                foodSpawnPoints.Add(child.gameObject.GetComponent<FoodSpawnPoint>());
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
            FoodSpawnPoint spawnpoints = foodSpawnPoints
                .OrderBy(_ => Random.value)
                .FirstOrDefault(spawnPoint => spawnPoint.IsEmpty);
            if (spawnpoints is not null) 
                spawnpoints.Spawn(GetRandomFood());
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

        private Foodbed GetRandomFood() => foodbeds
            .Where(bed => bed.CanSpawn(Random.value))
            .OrderBy(_ => Random.value)
            .First();

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