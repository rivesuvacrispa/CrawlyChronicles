using System.Collections;
using System.Collections.Generic;
using Definitions;
using Scriptable;
using Timeline;
using UI;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EnemyWaveSpawner : MonoBehaviour
    {
        [SerializeField] private Enemy neutral;
        [SerializeField] private EnemyList enemyList = new();
        [SerializeField] private Transform enemySpawnPointsTransform;
        [Header("Spawn rates")]
        [SerializeField] private float neutralsPerMinute;
        [SerializeField] private float enemyPerMinute;
        [SerializeField] private float enemyPerDayModifier;

        // Depends on difficulty
        private float currentEnemyPerMinute;
        private float currentEnemyPerDay;
        
        private static readonly List<EnemySpawnLocation> EnemySpawnPoints = new();
        public static int SpawnLocationsCount { get; private set; }
        
        
        private void Awake()
        {
            foreach (Transform child in enemySpawnPointsTransform)
                EnemySpawnPoints.Add(child.GetComponent<EnemySpawnLocation>());
            SpawnLocationsCount = EnemySpawnPoints.Count;
            SubToEvents();
        }

        private void Start() => OnDifficultyChanged(SettingsMenu.SelectedDifficulty);

        private void SpawnEnemy(Enemy toSpawn, EnemySpawnLocation location)
        {
            Enemy enemy = Instantiate(toSpawn, GlobalDefinitions.GameObjectsTransform);
            enemy.SpawnLocation = location;
            enemy.transform.position = location.SpawnPosition;
        }
        
        private IEnumerator SpawnRoutine()
        {
            yield return new WaitUntil(() => 
                EnemySpawnLocation.InitializedLocationsAmount == SpawnLocationsCount);

            while (enabled)
            {
                if (TimeManager.IsDay)
                {
                    SpawnEnemy(neutral, GetRandomSpawnPoint());
                    yield return new WaitForSeconds(60f / neutralsPerMinute);
                }
                else
                {
                    var day = TimeManager.Instance.DayCounter - 1;
                    SpawnEnemy(enemyList.GetRandomEnemyForDay(day), GetRandomSpawnPoint());
                    yield return new WaitForSeconds(60f / (currentEnemyPerMinute + currentEnemyPerDay * day));
                }
            }
        }

        private void OnEnable() => StartCoroutine(SpawnRoutine());
        
        private void OnResetRequested()
        {
            StopAllCoroutines();
            StartCoroutine(SpawnRoutine());
        }

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            float modifier = difficulty.EnemySpawnRate;
            currentEnemyPerMinute = enemyPerMinute * modifier;
            currentEnemyPerDay = enemyPerDayModifier * modifier;
        }
        
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


        public static EnemySpawnLocation GetRandomSpawnPoint() => EnemySpawnPoints[Random.Range(0, EnemySpawnPoints.Count)];
    }
}