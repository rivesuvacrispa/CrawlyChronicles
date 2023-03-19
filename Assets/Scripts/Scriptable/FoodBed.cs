using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Food bed")]
    public class FoodBed : ScriptableObject
    {
        [SerializeField] private bool dayGrown;
        [SerializeField] private Vector2Int amountRandom;
        [SerializeField] private Sprite[] growthSprites = new Sprite[6];

        public Sprite GetGrowthSprite(int amount)
        {
            if (amount > 5) amount = 5;
            return growthSprites[amount];
        }

        public bool DayGrown => dayGrown;
        public int GetRandomAmount() => Random.Range(amountRandom.x, amountRandom.y + 1);
    }
}