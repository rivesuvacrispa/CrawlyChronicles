using System;
using System.Text;
using UnityEngine;
using Util;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Difficulty")]
    public class Difficulty : ScriptableObject
    {
        [Header("Difficulty changes")]
        [SerializeField] private OverallDifficulty difficulty;
        [SerializeField] private string shortDescription;
        [SerializeField, Range(0, 2)] private float enemySpawnRate;
        [SerializeField, Range(0, 2)] private float foodSpawnRate;
        [SerializeField, Range(0, 2)] private float enemyStatsMultiplier;
        [SerializeField, Range(0, 2)] private float geneRerollCost;
        [SerializeField, Range(0, 2)] private float breedingCost;
        [SerializeField] private string bossString;

        public float EnemySpawnRate => enemySpawnRate;
        public float FoodSpawnRate => foodSpawnRate;
        public float EnemyStatsMultiplier => enemyStatsMultiplier;
        public float GeneRerollCost => geneRerollCost;
        public float BreedingCost => breedingCost;
        public OverallDifficulty OverallDifficulty => difficulty;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<b>").Append(shortDescription).Append("</b>").Append("\n");
            
            AddDescriptionString(sb, enemySpawnRate, "Enemy spawn rate");
            AddDescriptionString(sb, foodSpawnRate, "Food spawn rate", "red", "lime");
            AddEnemyStatsString(sb);
            AddDescriptionString(sb, geneRerollCost, "Gene reroll cost");
            AddDescriptionString(sb, breedingCost, "Breeding cost");
            sb.Append(bossString);
            return sb.ToString();
        }

        private void AddEnemyStatsString(StringBuilder sb)
        {
            bool less = enemyStatsMultiplier < 1;
            bool more = enemyStatsMultiplier > 1;
            if (less || more)
            {
                sb.Append("Enemies are ")
                    .Append(less ? "<color=lime>weaker</color>" : "<color=red>stronger</color>")
                    .Append(" by ")
                    .Append("<color=orange>")
                    .Append((int) (Math.Abs(1 - enemyStatsMultiplier) * 100))
                    .Append("%</color>");
            }
            else sb.Append("<color=orange>Default</color> enemies");
            sb.Append("\n");
        }
        
        private void AddDescriptionString(StringBuilder sb, float value, string title, string goodColor = "lime", string badColor = "red")
        {
            bool less = value < 1;
            bool more = value > 1;
            if (less || more)
            {
                sb.Append(title)
                    .Append(" is ")
                    .Append(less ? $"<color={goodColor}>decreased</color>" : $"<color={badColor}>increased</color>")
                    .Append(" by ")
                    .Append("<color=orange>")
                    .Append(Mathf.CeilToInt(Math.Abs(1 - value) * 100))
                    .Append("%</color>");
            }
            else sb.Append("<color=orange>Default</color> ").Append(title.ToLower());
            sb.Append("\n");
        }
    }
}