using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Food bed")]
    public class FoodBed : ScriptableObject
    {
        [SerializeField] private Vector2Int amountRandom;

        public int GetRandomAmount() => Random.Range(amountRandom.x, amountRandom.y + 1);
    }
}