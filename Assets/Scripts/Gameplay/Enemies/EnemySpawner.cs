using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private int enemyPerMinute;
        [SerializeField] private Transform enemySpawnPointsTransform;

        private readonly List<Vector3> enemySpawnPoints = new();
        
        private void Awake()
        {
            foreach (Transform child in enemySpawnPointsTransform)
                enemySpawnPoints.Add(child.localPosition);
        }
        
        private void SpawnEnemy(Vector3 vector3)
        {
            Enemy enemy = Instantiate(enemyPrefab);
            enemy.SetTarget(playerTransform);
            enemy.transform.position = vector3;
        }
        
        private IEnumerator SpawnRoutine()
        {
            while (enemyPerMinute > 0)
            {
                yield return new WaitForSeconds(60f / enemyPerMinute);
                SpawnEnemy(GetRandomSpawnPoint());
            }
        }

        private void OnDisable() => StopAllCoroutines();

        private void OnEnable() => StartCoroutine(SpawnRoutine());

        private Vector3 GetRandomSpawnPoint() => enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
    }
}