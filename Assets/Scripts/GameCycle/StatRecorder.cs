using System.Text;
using Gameplay.Breeding;
using Gameplay.Genes;
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
        public static int eggsLaid;
        public static int eggsLost;
        public static int timesBreed;
        public static int genesCollected;
        public static int timesMutated;

        private void OnEnable()
        {
            MainMenu.OnResetRequested += ResetStats;
            
            GeneDrop.OnPickedUp += OnGenePickup;
            BreedingManager.OnBecomePregnant += OnBecomePregnant;
            BreedingManager.OnEggsLaid += OnEggsLaid;
        }

        private void OnDisable()
        {
            MainMenu.OnResetRequested -= ResetStats;
            
            GeneDrop.OnPickedUp -= OnGenePickup;
            BreedingManager.OnBecomePregnant -= OnBecomePregnant;
        }

        private void ResetStats()
        {
            daysSurvived = 0;
            damageDealt = 0;
            enemyKills = 0;
            respawns = 0;
            eggsLaid = 0;
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
            sb.Append("Eggs layed").Append(": ").Append(eggsLaid).Append("\n");
            sb.Append("Eggs lost").Append(": ").Append(eggsLost).Append("\n");
            sb.Append("Times breed").Append(": ").Append(timesBreed).Append("\n");
            sb.Append("Times mutated").Append(": ").Append(timesMutated).Append("\n");
            sb.Append("Genes collected").Append(": ").Append(genesCollected);
            return sb.ToString();
        }

        private void OnGenePickup(GeneType type, int amount) => genesCollected += amount;

        private void OnBecomePregnant() => timesBreed++;
        private void OnEggsLaid(int amount) => eggsLaid += amount;
    }
}