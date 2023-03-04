using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Enemy spawner")]
    public class EnemySpawner : ScriptableObject
    {
        [SerializeField] private Gameplay.Enemies.Enemy enemy;
        [SerializeField] private int spawnTime;

        public Gameplay.Enemies.Enemy Enemy => enemy;
        public int SpawnTime => spawnTime;
    }
}