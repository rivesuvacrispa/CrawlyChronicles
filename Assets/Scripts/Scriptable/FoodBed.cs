using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Food bed")]
    public class FoodBed : ScriptableObject
    {
        [SerializeField] private Vector2Int amountRandom;
        [SerializeField] private Sprite[] growthSprites = new Sprite[6];

        public Sprite GetGrowthSprite(int amount)
        {
            return growthSprites[Mathf.Clamp(amount - 1, 0, 5)];
        }

        public int GetRandomAmount() => Random.Range(amountRandom.x, amountRandom.y + 1);
    }
}