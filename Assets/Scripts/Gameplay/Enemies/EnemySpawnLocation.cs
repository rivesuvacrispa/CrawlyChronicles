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
        public static int InitializedLocationsAmount { get; private set; }

        private void Awake()
        {
            SpawnPosition = transform.position;

            seeker.StartPath(SpawnPosition, exitTransform.position, enterPath =>
            {
                enterPath.Claim(this);
                EnteringPath = enterPath;
                InitializedLocationsAmount++;
            });
        }
    }
}