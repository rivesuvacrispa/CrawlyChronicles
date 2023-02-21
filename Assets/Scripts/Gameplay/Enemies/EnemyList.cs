using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Enemies
{
    [System.Serializable]
    public class EnemyList
    {
        [SerializeField] private List<EnemyDay> enemies = new();

        public Enemy GetRandomEnemyForDay(int day)
        {
            day = Mathf.Clamp(day, 0, enemies.Count - 1);
            return enemies[day].GetRandom();
        }
    }

    [System.Serializable]
    public class EnemyDay
    {
        [SerializeField] private List<Enemy> enemies = new();

        public Enemy GetRandom() => enemies[Random.Range(0, enemies.Count)];
    }
}