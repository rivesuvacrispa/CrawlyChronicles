using System;
using Gameplay.Map;
using Pathfinding;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EnemySpawnLocation : MonoBehaviour
    {
        [SerializeField] private Transform exitTransform;
        [SerializeField] private Seeker seeker;
    
    
        
        public Path EnteringPath { get; private set; }
        public Vector3 SpawnPosition { get; private set; }
        public bool Initialized { get; private set; }

        
        
        private void OnEnable() => MapManager.OnAfterMapLoad += OnMapLoad;

        private void OnDisable() => MapManager.OnAfterMapLoad -= OnMapLoad;

        private void OnMapLoad() => Initialized = false;

        private void Awake()
        {
            SpawnPosition = transform.position;

            seeker.StartPath(SpawnPosition, exitTransform.position, enterPath =>
            {
                enterPath.Claim(this);
                EnteringPath = enterPath;
                Initialized = true;
            });
        }
    }
}