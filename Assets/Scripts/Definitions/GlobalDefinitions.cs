using System;
using Gameplay;
using Gameplay.Genetics;
using UI;
using UnityEngine;
using Util;

namespace Definitions
{
    public class GlobalDefinitions : MonoBehaviour
    {
        private static GlobalDefinitions instance;
        [Header("Miscs")]
        [SerializeField] private Sprite[] eggSprites = new Sprite[6];
        [SerializeField] private Sprite puddleSprite;
        [SerializeField] private Transform gameObjectsTransform;
        [SerializeField] private Transform worldCanvasTransform;
        [Header("Stats")] 
        [SerializeField] private int eggGeneEntropy;
        [SerializeField] private int breedingPartnersGeneEntropy;
        [SerializeField] private float despawnTime;
        [SerializeField] private float wanderingSpeedMultiplier;
        [SerializeField] private float fleeingSpeedMultiplier;
        [SerializeField] private float interactionDistance;
        [SerializeField] private float genePickupDistance;
        [Header("Prefabs")]
        [SerializeField] private Egg eggPrefab;
        [SerializeField] private PopupNotification popupNotificationPrefab;
        [SerializeField] private GeneDrop geneDropPrefab;
        [Header("Colors")]
        [SerializeField] private Color deadColor;
        [SerializeField] private Color eggPuddleColor;
        [SerializeField] private Color aggressiveGeneColor;
        [SerializeField] private Color defensiveGeneColor;
        [SerializeField] private Color universalGeneColor;
        
        private static Gradient deathGradient;
        private static readonly Color[] geneColors = new Color[3];
        
        public static Transform GameObjectsTransform => instance.gameObjectsTransform;
        public static Transform WorldCanvasTransform => instance.worldCanvasTransform;
        public static float DespawnTime => instance.despawnTime;
        public static float WanderingSpeedMultiplier => instance.wanderingSpeedMultiplier;
        public static float FleeingSpeedMultiplier => instance.fleeingSpeedMultiplier;
        public static float InteractionDistance => instance.interactionDistance;
        public static int EnemyPhysicsLayerMask { get; private set; }
        public static Sprite PuddleSprite => instance.puddleSprite;
        public static Color EggPuddleColor => instance.eggPuddleColor;
        public static float GenePickupDistance => instance.genePickupDistance;
        public static int BreedingPartnersGeneEntropy => instance.breedingPartnersGeneEntropy;
        public static int EggGeneEntropy => instance.eggGeneEntropy;
        
        
        
        public static Sprite GetEggsBedSprite(int eggsAmount) => instance.eggSprites[Mathf.Clamp(eggsAmount - 1, 0, 5)];
        public static Color GetDeadColor(float t) => deathGradient.Evaluate(t);
        public static Color GetGeneColor(GeneType geneType) => geneColors[(int) geneType];
        
        
        
        public static Egg CreateEgg(TrioGene genes) =>
            Instantiate(instance.eggPrefab, instance.gameObjectsTransform)
                .SetGenes(genes);

        public static PopupNotification CreateNotification(INotificationProvider provider, bool isStatic = true) =>
            Instantiate(instance.popupNotificationPrefab, instance.worldCanvasTransform)
                .SetDataProvider(provider, isStatic);

        public static void CreateGeneDrop(Vector3 position, GeneType geneType) =>
            Instantiate(instance.geneDropPrefab, instance.gameObjectsTransform).SetGeneType(geneType)
                .transform.localPosition = position;

        

        private void Awake()
        {
            geneColors[0] = aggressiveGeneColor;
            geneColors[1] = defensiveGeneColor;
            geneColors[2] = universalGeneColor;
            EnemyPhysicsLayerMask = LayerMask.NameToLayer("EnemyPhysics");
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