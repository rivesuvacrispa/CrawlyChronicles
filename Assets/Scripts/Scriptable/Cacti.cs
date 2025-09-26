using UnityEngine;

namespace Scriptable
{
    public class Cacti : FoodBed
    {
        [SerializeField] private Sprite[] spikesOnEatenSprites;
        [SerializeField] private Sprite[] spikesOnTouchSprites;
        [SerializeField] private float contactDamage;
        [SerializeField] private float knockback;


        public Sprite GetSpikeOnEatenSprite(int amountLeft) => spikesOnEatenSprites[Mathf.Clamp(amountLeft - 1, 0, 4)];
        public Sprite GetSpikeOnTouchSprite(int amountLeft) => spikesOnTouchSprites[Mathf.Clamp(amountLeft - 1, 0, 3)];

        public float ContactDamage => contactDamage;
        public float Knockback => knockback;
    }
}