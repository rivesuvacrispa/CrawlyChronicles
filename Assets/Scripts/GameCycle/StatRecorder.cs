using System.Text;
using UI;
using UI.Menus;
using UnityEngine;

namespace GameCycle
{
    public class StatRecorder : MonoBehaviour
    {
        public static int daysSurvived;
        public static float damageDealt;
        public static int enemyKills;
        public static int respawns;
        public static int eggsLayed;
        public static int eggsLost;
        public static int timesBreed;
        public static int genesCollected;
        public static int timesMutated;

        private void Start() => MainMenu.OnResetRequested += ResetStats;

        private void OnDestroy() => MainMenu.OnResetRequested -= ResetStats;

        public static void ResetStats()
        {
            daysSurvived = 0;
            damageDealt = 0;
            enemyKills = 0;
            respawns = 0;
            eggsLayed = 0;
            eggsLost = 0;
            timesBreed = 0;
            genesCollected = 0;
            timesMutated = 0;
        }

        public static string Print()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Days survived").Append(": ").Append(daysSurvived).Append("\n");
            sb.Append("Damage dealt").Append(": ").Append(damageDealt.ToString("n2")).Append("\n");
            sb.Append("Enemy kills").Append(": ").Append(enemyKills).Append("\n");
            sb.Append("Respawns").Append(": ").Append(respawns).Append("\n");
            sb.Append("Eggs layed").Append(": ").Append(eggsLayed).Append("\n");
            sb.Append("Eggs lost").Append(": ").Append(eggsLost).Append("\n");
            sb.Append("Times breed").Append(": ").Append(timesBreed).Append("\n");
            sb.Append("Times mutated").Append(": ").Append(timesMutated).Append("\n");
            sb.Append("Genes collected").Append(": ").Append(genesCollected);
            return sb.ToString();
        }
    }
}