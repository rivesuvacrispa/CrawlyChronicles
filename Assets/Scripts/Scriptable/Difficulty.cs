using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using Util;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Difficulty")]
    public class Difficulty : ScriptableObject
    {
        [Header("Difficulty changes")]
        [SerializeField] private OverallDifficulty difficulty;
        [SerializeField, Range(0, 2)] private float enemySpawnRate;
        [SerializeField, Range(0, 2)] private float foodSpawnRate;
        [SerializeField, Range(0, 2)] private float enemyStatsMultiplier;
        [SerializeField, Range(0, 2)] private float geneRerollCost;
        [SerializeField, Range(0, 2)] private float breedingCost;
        [SerializeField, Range(0, 1)] private float enemiesStrongerPerDay;

        public OverallDifficulty OverallDifficulty => difficulty;
        public float EnemySpawnRate => enemySpawnRate;
        public float FoodSpawnRate => foodSpawnRate;
        public float EnemyStatsMultiplier => enemyStatsMultiplier;
        public float GeneRerollCost => geneRerollCost;
        public float BreedingCost => breedingCost;
        public float EnemiesStrongerPerDay => enemiesStrongerPerDay;

        private TableEntryReference descriptionReference;
        private TableEntryReference nameReference;
        private static readonly TableReference TableReference = "Difficulties";
        
        

        public string GetDescription()
        {
            var args = new object[]
            {
                Mathf.RoundToInt((enemySpawnRate - 1) * 100),
                Mathf.RoundToInt((foodSpawnRate - 1) * 100),
                Mathf.RoundToInt((enemyStatsMultiplier - 1) * 100),
                Mathf.RoundToInt((geneRerollCost - 1) * 100),
                Mathf.RoundToInt((breedingCost - 1) * 100),
                Mathf.RoundToInt(enemiesStrongerPerDay * 100),
            };
            return LocalizationSettings.StringDatabase.GetLocalizedString(TableReference, descriptionReference, args);
        }

        public string GetName() => LocalizationSettings.StringDatabase.GetLocalizedString(TableReference, nameReference);
        
        private void Init()
        {
            nameReference = $"Name_{name}";
            descriptionReference = $"Description_{name}";
        }

        private void OnValidate() => Init();
        private void Awake() => Init();
    }
}