using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Enemy wave")]
    public class EnemyWave : ScriptableObject
    {
        [System.Serializable]
        private class WaveEnemy
        {
            [SerializeField] private Gameplay.Enemies.Enemy prefab;
            [SerializeField, Range(0, 1)] private float spawnRate;
            [SerializeField, Range(0, 1), ShowOnly] private float actualSpawnRate = 1;

            public void CorrectSpawnRate(float spawnRatesSum)
            {
                if (spawnRatesSum == 0) spawnRatesSum = 1;
                actualSpawnRate = spawnRate / spawnRatesSum;
            }

            public Gameplay.Enemies.Enemy Prefab => prefab;
            public float ActualSpawnRate => actualSpawnRate;
            public float SpawnRate => spawnRate;

            public WaveEnemy(Gameplay.Enemies.Enemy prefab, float spawnRate)
            {
                this.prefab = prefab;
                this.spawnRate = spawnRate;
            }
        }


        
        [SerializeField] private int canSpawnFromDay;
        [SerializeField] private List<WaveEnemy> enemies = new();

        public int CanSpawnFromDay => canSpawnFromDay;

        public Gameplay.Enemies.Enemy GetRandomEnemy()
        {
            if (enemies.Count == 1) return enemies[0].Prefab;
            
            float step = 0;
            float rnd = Random.value;
            foreach (WaveEnemy e in enemies)
            {
                step += e.ActualSpawnRate;
                if(rnd <= step) return e.Prefab;
            }

            return null;
        }
        
        private void OnValidate()
        {
            float sum = enemies.Sum(enemy => enemy.SpawnRate);
            foreach (WaveEnemy enemy in enemies) 
                enemy.CorrectSpawnRate(sum);
        }
    }
}