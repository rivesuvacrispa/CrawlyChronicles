using Genes;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Enemy")]
    public class Enemy : ScriptableObject
    {
        [Header("Utility fields")]
        [SerializeField] private string animatorName;
        [SerializeField] private float healthbarOffsetY;
        [SerializeField] private float healthbarWidth;

        [Header("Stats fields")] 
        [SerializeField, Range(0, 1)] private float geneDropRate;
        [SerializeField] private float immunityDuration;
        [SerializeField] private float maxHealth;
        [SerializeField] private int wanderingRadius;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float locatorRadius;
        [SerializeField] private Color bodyColor;
        [SerializeField] private float damage;
        [SerializeField] private float knockback;

        private Gradient immunityGradient;

        public string AnimatorName => animatorName;
        public float ImmunityDuration => immunityDuration;
        public float MaxHealth => maxHealth;
        public Color BodyColor => bodyColor;
        public float HealthbarOffsetY => healthbarOffsetY;
        public float HealthbarWidth => healthbarWidth;
        public int WanderingRadius => wanderingRadius;
        public float MovementSpeed => movementSpeed;
        public float LocatorRadius => locatorRadius;
        public float GeneDropRate => geneDropRate;

        public float Damage => damage;

        public float Knockback => knockback;


        public Color GetImmunityFrameColor(float time) => immunityGradient.Evaluate(time / immunityDuration);
        
        
        
        private void Awake()
        {
            immunityGradient = new Gradient();
            immunityGradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0),
                    new GradientColorKey(bodyColor, 1)
                }, new[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1),
                });
        }
    }
}