using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scriptable;
using Timeline;
using UI;
using UnityEngine;

namespace Gameplay.Food
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private FoodBed firstDayDood;
        [SerializeField] private int startingFoodAmount;
        [SerializeField] private List<FoodBed> foodPrefabs;
        [SerializeField] private Transform foodSpawnPointsTransform;
        [SerializeField] private float foodPerMinute;

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
            StartCoroutine(FoodSpawningRoutine());
        }

        private void OnResetRequested()
        {
            for (int i = 0; i < startingFoodAmount + (int) SettingsMenu.SelectedDifficulty.OverallDifficulty; i++)
                SpawnFood();
        }

        private void SpawnFood()
        {
            FoodSpawnPoint foodToSpawn = foodSpawnPoints
                .OrderBy(_ => Random.value)
                .FirstOrDefault(spawnPoint => spawnPoint.IsEmpty);
            if (foodToSpawn is not null) 
                foodToSpawn.Spawn(GetRandomFood());
        }

        private IEnumerator FoodSpawningRoutine()
        {
            while (currentFoodPerMinute > 0)
            {
                float delay = 60f / currentFoodPerMinute;
                delay -= delay * Random.value * 0.3f - 0.15f;
                yield return new WaitForSeconds(delay);
                
                SpawnFood();
            }
        }

        private FoodBed GetRandomFood()
        {
            if (TimeManager.Instance.DayCounter <= 1) return firstDayDood;
            return foodPrefabs[Random.Range(0, foodPrefabs.Count)];
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