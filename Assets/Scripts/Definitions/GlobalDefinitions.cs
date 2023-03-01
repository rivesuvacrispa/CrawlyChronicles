using Gameplay;
using Gameplay.Enemies;
using Genes;
using UI;
using UnityEngine;
using Util;

namespace Definitions
{
    public class GlobalDefinitions : MonoBehaviour
    {
        private static GlobalDefinitions instance;
        [Header("Miscs")] 
        [SerializeField] private int mutationCostPerLevel;
        [SerializeField] private string[] romanDigits = new string[10];
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
        [SerializeField] private SandFunnel sandFunnel;
        [SerializeField] private EggBed eggBedPrefab;
        [SerializeField] private EggDrop eggDropPrefab;
        [SerializeField] private PopupNotification popupNotificationPrefab;
        [SerializeField] private GeneDrop geneDropPrefab;
        [Header("Colors")]
        [SerializeField] private Color deadColor;
        [SerializeField] private Color eggPuddleColor;
        [SerializeField] private Color[] geneColors = new Color[3];
        [SerializeField] private Color poisonColor;
        [Header("Physics")]
        [SerializeField] private float playerMass = 0.175f;
        [SerializeField] private float maxAppliedForce = 5;
        [SerializeField] private float enemyImmunityDuration = 0.5f;
        
        
        private static Gradient deathGradient;
        
        
        public static Transform GameObjectsTransform => instance.gameObjectsTransform;
        public static Transform WorldCanvasTransform => instance.worldCanvasTransform;
        public static float DespawnTime => instance.despawnTime;
        public static float WanderingSpeedMultiplier => instance.wanderingSpeedMultiplier;
        public static float FleeingSpeedMultiplier => instance.fleeingSpeedMultiplier;
        public static float InteractionDistance => instance.interactionDistance;
        public static int EnemyPhysicsLayerMask { get; private set; }
        public static int EnemyAttackLayerMask { get; private set; }
        public static int PlayerAttackLayerMask { get; private set; }
        public static Sprite PuddleSprite => instance.puddleSprite;
        public static Color EggPuddleColor => instance.eggPuddleColor;
        public static float GenePickupDistance => instance.genePickupDistance;
        public static int BreedingPartnersGeneEntropy => instance.breedingPartnersGeneEntropy;
        public static int EggGeneEntropy => instance.eggGeneEntropy;
        public static EggBed EggBedPrefab => instance.eggBedPrefab;
        public static float MaxAppliableForce => instance.maxAppliedForce;
        public static float PlayerMass => instance.playerMass;
        public static float EnemyImmunityDuration => instance.enemyImmunityDuration;
        public static Color PoisonColor => instance.poisonColor;
        public static Color DeadColor => instance.deadColor;
        public static Gradient DeathGradient => deathGradient;


        public static string GetRomanDigit(int digit) => instance.romanDigits[digit];
        public static Sprite GetEggsBedSprite(int eggsAmount) => instance.eggSprites[Mathf.Clamp(eggsAmount - 1, 0, 5)];
        public static Color GetGeneColor(GeneType geneType) => instance.geneColors[(int) geneType];
        public static int GetMutationCost(int lvl) => (lvl + 1) * instance.mutationCostPerLevel;
        

        
        public static SandFunnel CreateSandFunnel(Vector2 position)
        {
            var funnel = Instantiate(instance.sandFunnel, instance.gameObjectsTransform);
            funnel.transform.position = position;
            funnel.enabled = true;
            return funnel;
        }
        
        public static EggDrop CreateEggDrop(Egg egg)=>
            Instantiate(instance.eggDropPrefab, instance.gameObjectsTransform)
                .SetEgg(egg);
        
        public static void CreateEggSquash(Vector3 pos)
        {
            var egg = Instantiate(instance.eggDropPrefab, instance.gameObjectsTransform);
            egg.Squash();
            egg.transform.position = pos;
        }

        public static PopupNotification CreateNotification(INotificationProvider provider, bool isStatic = true) =>
            Instantiate(instance.popupNotificationPrefab, instance.worldCanvasTransform)
                .SetDataProvider(provider, isStatic);

        public static void CreateGeneDrop(Vector3 position, GeneType geneType, int amount = 1) =>
            Instantiate(instance.geneDropPrefab, instance.gameObjectsTransform)
                .SetData(geneType, amount)
                    .transform.localPosition = position;

        public static void CreateRandomGeneDrop(Vector3 position) =>
            CreateGeneDrop(position, (GeneType) Random.Range(0, 3));

        private GlobalDefinitions()
        {
            instance = this;
        }

        private void Awake()
        {
            EnemyPhysicsLayerMask = LayerMask.NameToLayer("EnemyPhysics");
            EnemyAttackLayerMask = LayerMask.NameToLayer("EnemyAttacks");
            PlayerAttackLayerMask = LayerMask.NameToLayer("PlayerAttacks");
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