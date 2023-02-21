using System.Collections;
using System.Collections.Generic;
using Definitions;
using Timeline;
using UI;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy neutral;
        [SerializeField] private EnemyList enemyList = new();
        [SerializeField] private int neutralsPerMinute = 6;
        [SerializeField] private int enemyPerMinute;
        [SerializeField] private int enemyPerDayModifier = 10;
        [SerializeField] private Transform enemySpawnPointsTransform;

        private readonly List<EnemySpawnLocation> enemySpawnPoints = new();
        
        public static int SpawnLocationsCount { get; private set; }
        
        
        private void Awake()
        {
            foreach (Transform child in enemySpawnPointsTransform)
                enemySpawnPoints.Add(child.GetComponent<EnemySpawnLocation>());
            SpawnLocationsCount = enemySpawnPoints.Count;
            MainMenu.OnResetRequested += OnResetRequested;
        }
        
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
                    SpawnEnemy(enemyList.GetRandomEnemyForDay(TimeManager.Instance.DayCounter), GetRandomSpawnPoint());
                    yield return new WaitForSeconds(60f / (enemyPerMinute + enemyPerDayModifier));
                }
                
            }
        }

        private void OnEnable() => StartCoroutine(SpawnRoutine());
        
        private void OnResetRequested()
        {
            StopAllCoroutines();
            StartCoroutine(SpawnRoutine());
        }
        private void OnDestroy() => MainMenu.OnResetRequested -= OnResetRequested;
        

        private EnemySpawnLocation GetRandomSpawnPoint() => enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
    }
}