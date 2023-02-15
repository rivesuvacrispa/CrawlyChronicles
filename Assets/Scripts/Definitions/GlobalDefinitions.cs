using UI;
using UnityEngine;
using Util;

namespace Definitions
{
    public class GlobalDefinitions : MonoBehaviour
    {
        private static GlobalDefinitions instance;

        [SerializeField] private PopupNotification popupNotificationPrefab;
        [SerializeField] private Sprite[] eggSprites = new Sprite[6];
        [SerializeField] private Color deadColor;
        [SerializeField] private float despawnTime;
        [SerializeField] private Transform gameObjectsTransform;
        [SerializeField] private Transform worldCanvasTransform;
        [SerializeField] private float wanderingSpeedMultiplier;
        [SerializeField] private float fleeingSpeedMultiplier;
        [SerializeField] private float interactionDistance;
        
        private static Gradient deathGradient;
        
        public static Transform GameObjectsTransform => instance.gameObjectsTransform;
        public static Transform WorldCanvasTransform => instance.worldCanvasTransform;
        public static Sprite GetEggsBedSprite(int eggsAmount) => instance.eggSprites[Mathf.Clamp(eggsAmount - 1, 0, 6)];
        public static Color GetDeadColor(float t) => deathGradient.Evaluate(t);
        public static float DespawnTime => instance.despawnTime;
        public static float WanderingSpeedMultiplier => instance.wanderingSpeedMultiplier;
        public static float FleeingSpeedMultiplier => instance.fleeingSpeedMultiplier;
        public static float InteractionDistance => instance.interactionDistance;
        

        public static void CreateNotification(INotificationProvider provider) =>
            Instantiate(instance.popupNotificationPrefab, instance.worldCanvasTransform)
                .SetDataProvider(provider);

        private void Awake()
        {
            instance = this;
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