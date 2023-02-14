using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Enemy")]
    public class Enemy : ScriptableObject
    {
        [SerializeField] private string animatorName;
        [SerializeField] private float immunityDuration;
        [SerializeField] private int maxHealth;
        [SerializeField] private int healthRegen;
        [SerializeField] private float healthbarOffsetY;
        [SerializeField] private float healthbarWidth;
        [SerializeField] private Color color;

        private Gradient immunityGradient;

        public string AnimatorName => animatorName;
        public float ImmunityDuration => immunityDuration;
        public int MaxHealth => maxHealth;
        public int HealthRegen => healthRegen;
        public Color Color => color;
        public float HealthbarOffsetY => healthbarOffsetY;
        public float HealthbarWidth => healthbarWidth;


        public Color GetImmunityFrameColor(float time) => immunityGradient.Evaluate(time / immunityDuration);
        
        private void Awake()
        {
            immunityGradient = new Gradient();
            immunityGradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0),
                    new GradientColorKey(color, 1)
                }, new[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1),
                });
        }
    }
}