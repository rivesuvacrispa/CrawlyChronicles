using UnityEngine;

namespace Definitions
{
    public class GlobalDefinitions : MonoBehaviour
    {
        [SerializeField] private Color deadColor;
        [SerializeField] private float despawnTime;
        
        private static Gradient deathGradient;
        
        public static Color GetDeadColor(float t) => deathGradient.Evaluate(t);
        public static float DespawnTime { get; private set; }

        private void Awake()
        {
            DespawnTime = despawnTime;
            deathGradient = new Gradient();
            deathGradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0),
                    new GradientColorKey(deadColor, 1)
                }, new[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1),
                });
        }
    }
}