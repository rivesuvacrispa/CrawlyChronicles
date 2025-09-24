using System.Collections.Generic;
using System.Linq;
using Gameplay.Enemies;
using UnityEngine;

namespace Gameplay.Map
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private Transform enemySpawnPointsTransform;

        
        
        public static readonly List<EnemySpawnLocation> EnemySpawnPoints = new();
        public static int SpawnLocationsCount { get; private set; }
        public static EnemySpawnLocation GetRandomSpawnPoint() => EnemySpawnPoints[Random.Range(0, EnemySpawnPoints.Count)];

        public delegate void MapManagerEvent();
        public static event MapManagerEvent OnAfterMapLoad;
        public static bool AllSpawnPointsInitialized => EnemySpawnPoints.All(loc => loc.Initialized);
        
        
        
        private void Awake()
        {
            InitEnemySpawnPoints();
            
            OnAfterMapLoad?.Invoke();
        }

        private void InitEnemySpawnPoints()
        {
            EnemySpawnPoints.Clear();
            foreach (Transform child in enemySpawnPointsTransform)
                EnemySpawnPoints.Add(child.GetComponent<EnemySpawnLocation>());
            SpawnLocationsCount = EnemySpawnPoints.Count;
        }
    }
}