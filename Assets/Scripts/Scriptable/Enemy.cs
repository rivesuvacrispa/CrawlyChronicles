using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private float immunityDuration;
        [SerializeField] private int maxHealth;
        [SerializeField] private int healthRegen;
        [SerializeField] private int wanderingRadius;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float locatorRadius;
        [SerializeField] private Color bodyColor;

        private Gradient immunityGradient;

        public string AnimatorName => animatorName;
        public float ImmunityDuration => immunityDuration;
        public int MaxHealth => maxHealth;
        public int HealthRegen => healthRegen;
        public Color BodyColor => bodyColor;
        public float HealthbarOffsetY => healthbarOffsetY;
        public float HealthbarWidth => healthbarWidth;
        public int WanderingRadius => wanderingRadius;
        public float MovementSpeed => movementSpeed;
        public float LocatorRadius => locatorRadius;


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