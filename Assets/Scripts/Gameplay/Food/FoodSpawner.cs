using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Food
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> foodPrefabs;
        [SerializeField] private Transform foodSpawnPointsTransform;
        [SerializeField] private float foodPerMinute;

        private readonly List<FoodSpawnPoint> foodSpawnPoints = new();

        private void Awake()
        {
            foreach (Transform child in foodSpawnPointsTransform)
                foodSpawnPoints.Add(child.gameObject.GetComponent<FoodSpawnPoint>());
        }

        private void Start()
        {
            StartCoroutine(FoodSpawningRoutine());
        }

        private void SpawnFood()
        {
            FoodSpawnPoint spawn = foodSpawnPoints
                .OrderBy(_ => Random.value)
                .FirstOrDefault(spawnPoint => spawnPoint.IsEmpty);
            if (spawn is not null)
            {
                spawn.Spawn(GetRandomFood());
            };
        }

        private IEnumerator FoodSpawningRoutine()
        {
            while (foodPerMinute > 0)
            {
                float delay = 60f / foodPerMinute;
                delay -= delay * Random.value * 0.3f - 0.15f;
                yield return new WaitForSeconds(delay);
                
                SpawnFood();
            }
        }

        private GameObject GetRandomFood() => foodPrefabs[Random.Range(0, foodPrefabs.Count)];
    }
}