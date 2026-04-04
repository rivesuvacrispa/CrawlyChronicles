using Gameplay.Food.Foodbeds;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Food bed")]
    public class FoodBed : ScriptableObject
    {
        [SerializeField] private Vector2Int amountRandom;
        [SerializeField] private Sprite[] growthSprites = new Sprite[6];
        [SerializeField] private TimeOfDay timeOfDay = TimeOfDay.ANY;
        [SerializeField, Range(-1, 20)] private int canSpawnFromDay = -1; 
        [SerializeField, Range(0, 1)] private float spawnChance = 1f;


        public TimeOfDay TimeOfDay => timeOfDay;
        public float SpawnChance => spawnChance;
        public int CanSpawnFromDay => canSpawnFromDay;

        public Sprite GetGrowthSprite(int amount)
        {
            return growthSprites[Mathf.Clamp(amount - 1, 0, 5)];
        }

        public int GetRandomAmount() => Random.Range(amountRandom.x, amountRandom.y + 1);
        public int GetMaxAmount() => amountRandom.y;
    }
}