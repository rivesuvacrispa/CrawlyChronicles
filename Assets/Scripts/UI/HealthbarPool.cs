using Gameplay.Enemies;
using UnityEngine;

namespace UI
{
    public class HealthbarPool : MonoBehaviour
    {
        public static HealthbarPool Instance { get; private set; }
        
        [SerializeField] private HealthBar healthBarPrefab;

        private void Awake() => Instance = this;

        public HealthBar Create(Enemy enemy)
        {
            HealthBar hb = Instantiate(healthBarPrefab, transform);
            hb.SetTarget(enemy);
            return hb;
        }
    }
}