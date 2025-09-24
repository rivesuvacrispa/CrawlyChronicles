using System.Collections.Generic;
using System.Linq;
using Gameplay.Enemies;
using Gameplay.Food;
using UnityEngine;

namespace Gameplay.Map
{
    public class MapManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform enemySpawnPointsTransform;
        [SerializeField] private Transform foodSpawnPointsTransform;
        [Header("Settings")]
        [SerializeField] private List<Foodbed> foodbeds;

        private static MapManager instance;
        private static readonly List<EnemySpawnLocation> EnemySpawnPoints = new();
        private static readonly List<FoodSpawnPoint> FoodSpawnPoints = new();
        public delegate void MapManagerEvent();
        public static event MapManagerEvent OnAfterMapLoad;
        public static bool AllSpawnPointsInitialized => EnemySpawnPoints.All(loc => loc.Initialized);
        
        
        
        private void Awake()
        {
            instance = this;
            InitEnemySpawnPoints();
            InitFoodSpawnPoints();
            
            OnAfterMapLoad?.Invoke();
        }

        private void InitEnemySpawnPoints()
        {
            EnemySpawnPoints.Clear();
            foreach (Transform child in enemySpawnPointsTransform)
                EnemySpawnPoints.Add(child.GetComponent<EnemySpawnLocation>());
        }

        private void InitFoodSpawnPoints()
        {
            FoodSpawnPoints.Clear();
            foreach (Transform child in foodSpawnPointsTransform)
                FoodSpawnPoints.Add(child.gameObject.GetComponent<FoodSpawnPoint>());
        }
        
        public static EnemySpawnLocation GetRandomSpawnPoint() => EnemySpawnPoints[Random.Range(0, EnemySpawnPoints.Count)];

        public static bool TryGetFoodSpawnPoint(out FoodSpawnPoint spawnPoint)
        {
            spawnPoint = FoodSpawnPoints
                .OrderBy(_ => Random.value)
                .FirstOrDefault(spawnPoint => spawnPoint.IsEmpty);

            return spawnPoint is not null;
        }

        public static Foodbed GetRandomFood() => instance.foodbeds
            .Where(bed => bed.CanSpawn(Random.value))
            .OrderBy(_ => Random.value)
            .First();
    }
}