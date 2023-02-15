using System.Collections;
using System.Collections.Generic;
using Definitions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [FormerlySerializedAs("enemyAIPrefab")] [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private int enemyPerMinute;
        [SerializeField] private Transform enemySpawnPointsTransform;

        private readonly List<EnemySpawnLocation> enemySpawnPoints = new();
        
        public static int SpawnLocationsCount { get; private set; }
        
        private void Awake()
        {
            foreach (Transform child in enemySpawnPointsTransform)
                enemySpawnPoints.Add(child.GetComponent<EnemySpawnLocation>());
            SpawnLocationsCount = enemySpawnPoints.Count;
        }
        
        private void SpawnEnemy(EnemySpawnLocation location)
        {
            Enemy enemy = Instantiate(enemyPrefab, GlobalDefinitions.GameObjectsTransform);
            enemy.SpawnLocation = location;
            enemy.transform.position = location.SpawnPosition;
        }
        
        private IEnumerator SpawnRoutine()
        {
            yield return new WaitUntil(() => 
                EnemySpawnLocation.InitializedLocationsAmount == SpawnLocationsCount);
            
            while (enemyPerMinute > 0)
            {
                yield return new WaitForSeconds(60f / enemyPerMinute);
                SpawnEnemy(GetRandomSpawnPoint());
            }
        }

        private void OnDisable() => StopAllCoroutines();

        private void OnEnable() => StartCoroutine(SpawnRoutine());

        private EnemySpawnLocation GetRandomSpawnPoint() => enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
    }
}